import { useTranslation } from 'react-i18next';
import { Globe } from 'lucide-react';

export default function LanguageSwitcher() {
  const { i18n } = useTranslation();

  const toggleLanguage = () => {
    const newLang = i18n.language === 'es' ? 'en' : 'es';
    i18n.changeLanguage(newLang);
  };

  return (
    <button 
      onClick={toggleLanguage}
      className="flex items-center gap-2 bg-white/80 backdrop-blur-md px-4 py-2 rounded-full shadow-sm hover:shadow-md transition-all border border-gray-100 text-gray-600 hover:text-blue-600 font-medium text-sm cursor-pointer"
    >
      <Globe className="w-4 h-4" />
      {i18n.language === 'es' ? 'EN' : 'ES'}
    </button>
  );
}
