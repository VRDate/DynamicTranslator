﻿using System.Threading.Tasks;

namespace DynamicTranslator.Orchestrators
{
    #region using

    

    #endregion

    public interface INotifier
    {
        void AddNotification(string title, string imageUrl, string text);

        Task AddNotificationAsync(string title, string imageUrl, string text);
    }
}