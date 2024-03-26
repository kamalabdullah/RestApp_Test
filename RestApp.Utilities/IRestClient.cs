using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.Utilities
{
    public interface IRestClient
    {
        public Task<TModel> Get<TModel>(string url);
        public Task<TModel> Put<TModel>(string url, TModel model);
        public Task<TModel> Post<TModel>(string url, TModel model);
        public Task<TModel> Delete<TModel>(int id);
    }
}
