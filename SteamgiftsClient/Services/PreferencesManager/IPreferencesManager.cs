using System;
using System.Threading.Tasks;

namespace SteamgiftsClient.Services.PreferencesManager
{
    public interface IPreferencesManager<PreferencesData>
    {
        protected void InitAsync();

        /// <summary>
        /// Get copy of the preferences. Make changes to returned data does not affect to the current preferences
        /// </summary>
        /// <returns></returns>
        public PreferencesData GetPreferences();

        /// <summary>
        /// Async get copy of the preferences. Make changes to returned data does not affect to the current preferences
        /// </summary>
        /// <returns></returns>
        public Task<PreferencesData> GetPreferencesAsync();

        /// <summary>
        /// Get default preferences
        /// </summary>
        /// <returns></returns>
        public PreferencesData GetDefaultPreferences();

        /// <summary>
        /// Update current preferences by <paramref name="preferencesData"/>
        /// </summary>
        /// <param name="preferencesData"></param>
        /// <returns></returns>
        public Task UpdatePreferencesAsync(PreferencesData preferencesData);

        /// <summary>
        /// Make changes to current preferences by <paramref name="changesToMake"/>
        /// </summary>
        /// <param name="changesToMake"></param>
        /// <returns></returns>
        public Task MakeChangesAsync(Action<PreferencesData> changesToMake);

        /// <summary>
        /// Reset preferences to default
        /// </summary>
        /// <returns></returns>
        public Task ResetPreferencesAsync() => UpdatePreferencesAsync(GetDefaultPreferences());
    }
}
