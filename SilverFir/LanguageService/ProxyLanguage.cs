﻿namespace SilverFir.LanguageService
{
    public static class ProxyLanguage
    {
        public delegate ILanguageService? ProxyLanguageResolver(LanguageEnum language);
    }
}