using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace ConfiguratorMWS.Resources
{
    public class LocalizedStrings : INotifyPropertyChanged
    {
        private static ResourceManager _resourceManager =
           new ResourceManager("ConfiguratorMWS.Resources.Resources", typeof(LocalizedStrings).Assembly);

        private static CultureInfo _currentCulture;

        // Словарь для преобразования пользовательских кодов в стандартные
        private static readonly Dictionary<string, string> _cultureMap = new()
        {
            { "ENG", "en-US" },
            { "РУС", "ru-RU" },
            { "EN", "en-US" },
            { "RU", "ru-RU" }
        };

        public static CultureInfo CurrentCulture
        {
            get => _currentCulture ?? CultureInfo.CurrentUICulture;
            set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    OnLanguageChanged();
                }
            }
        }

        public string this[string key] => _resourceManager.GetString(key, CurrentCulture);

        public static event Action LanguageChanged;

        private static void OnLanguageChanged()
        {
            LanguageChanged?.Invoke();
        }

        public static void SetLanguage(string cultureName)
        {
            try
            {
                // Преобразуем пользовательский код в стандартный
                if (_cultureMap.TryGetValue(cultureName.ToUpper(), out string? standardCulture))
                {
                    CurrentCulture = new CultureInfo(standardCulture);
                }
                else
                {
                    throw new ArgumentException($"Unsupported language code: {cultureName}");
                }

                LanguageChanged?.Invoke(); // Уведомляем об изменении языка
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Error setting language: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}


