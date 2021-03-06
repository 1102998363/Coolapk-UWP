﻿using CoolapkUWP.Helpers;
using CoolapkUWP.Helpers.Providers;
using CoolapkUWP.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CoolapkUWP.ViewModels.UserListPage
{
    internal class ViewModel : IViewModel
    {
        private CoolapkListProvider provider;
        public ObservableCollection<Entity> Models { get => provider?.Models ?? null; }

        public double[] VerticalOffsets { get; set; } = new double[1];
        public string Title { get; }

        internal ViewModel(string uid, bool isFollowList, string name)
        {
            if (string.IsNullOrEmpty(uid)) { throw new ArgumentException(nameof(uid)); }

            Title = $"{name}的{(isFollowList ? "关注" : "粉丝")}";
            provider =
                new CoolapkListProvider(
                    async (p, page, firstItem, lastItem) =>
                    (JArray)await DataHelper.GetDataAsync(
                        DataUriType.GetUserList,
                        p == -2 ? true : false,
                        isFollowList ? "followList" : "fansList",
                        uid,
                        p < 0 ? ++page : p,
                        string.IsNullOrEmpty(firstItem) ? string.Empty : $"&firstItem={firstItem}",
                        string.IsNullOrEmpty(lastItem) ? string.Empty : $"&lastItem={lastItem}"),
                    (a, b) => ((UserModel)a).UserName == b.Value<string>(isFollowList ? "fusername" : "username"),
                    (o) => new Entity[] { new UserModel((JObject)(isFollowList ? o["fUserInfo"] : o["userInfo"])) },
                    "fuid");
        }

        public async Task Refresh(int p = -1) => await provider?.Refresh(p);
    }
}