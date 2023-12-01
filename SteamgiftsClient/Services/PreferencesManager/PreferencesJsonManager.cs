using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SteamgiftsClient.Services.PreferencesManager
{
    public class PreferencesJsonManager<PreferencesData> : IPreferencesManager<PreferencesData>
        where PreferencesData : class, new()
    {
        private string _preferencesFilePath;

        private PreferencesData? _defaultPreferences;

        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public PreferencesJsonManager(string preferencesFileName)
        {
            _preferencesFilePath = Path.Combine(App.DataFolder, preferencesFileName);
            InitAsync();
        }

        public async void InitAsync()
        {
            if (!File.Exists(_preferencesFilePath))
            {
                await UpdatePreferencesAsync(GetDefaultPreferences());
            }
        }

        public PreferencesData GetDefaultPreferences()
        {
            return _defaultPreferences ?? new PreferencesData();
        }

        public PreferencesData GetPreferences()
        {
            return Task.Run(async () => await GetPreferencesAsync()).Result;
        }

        public async Task<PreferencesData> GetPreferencesAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (File.Exists(_preferencesFilePath))
                {
                    using (StreamReader sr = File.OpenText(_preferencesFilePath))
                    {
                        return JsonConvert.DeserializeObject<PreferencesData>(await sr.ReadToEndAsync()) ?? GetDefaultPreferences();
                    }
                }
            }
            catch //(Exception exception)
            {
                //_logger.LogError(exception, "Failed to get user preferences");
            }
            finally
            {
                _semaphore.Release();
            }

            return GetDefaultPreferences();
        }

        public async Task MakeChangesAsync(Action<PreferencesData> changesToMake)
        {
            var preferences = await GetPreferencesAsync();
            changesToMake.Invoke(preferences);
            await UpdatePreferencesAsync(preferences);
        }

        public async Task UpdatePreferencesAsync(PreferencesData preferencesData)
        {
            await _semaphore.WaitAsync();
            try
            {
                string data = JsonConvert.SerializeObject(preferencesData, Newtonsoft.Json.Formatting.Indented);
                await File.WriteAllTextAsync(_preferencesFilePath, data);
            }
            catch //(Exception exception)
            {
                //_logger.LogError(exception, "Failed to update user preferences");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void SetDefaultPreferences(PreferencesData data)
        {
            _defaultPreferences = data;
        }
    }
}
