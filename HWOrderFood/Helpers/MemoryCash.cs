using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HWOrderFood.Helpers
{
    public class MemoryCash<TKey, TValue>
    {
        private readonly TimeSpan _time;
        private readonly Dictionary<TKey, Tuple<TValue, DateTime>> _dict = new Dictionary<TKey, Tuple<TValue, DateTime>>();
        private readonly Dictionary<TKey, Task<TValue>> _tasksDict = new Dictionary<TKey, Task<TValue>>();

        public MemoryCash(TimeSpan time)
        {
            _time = time;
        }

        public async Task<TValue> GetAsync(TKey key, Func<Task<TValue>> valFunc)
        {
            if (CanGetValue(key))
                return Get(key);
            else
            {
                if (_tasksDict.ContainsKey(key))
                    return await _tasksDict[key];
                else
                {
                    var t = valFunc();
                    _tasksDict[key] = t;
                    try
                    {
                        var res = await t;
                        Set(key, res);
                        return res;
                    }
                    catch(Exception ex)
                    {
                        ErrorsTracker.TrackError(ex, ex.Message);
                        throw;
                    }
                    finally
                    {
                        _tasksDict.Remove(key);
                    }
                }
            }
            
        }

        #region -- Private helpers -- 

        private bool CanGetValue(TKey key)
        {
            if (_dict.ContainsKey(key))
            {
                var t = _dict[key];
                if (DateTime.Now - t.Item2 < _time)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private TValue Get(TKey key)
        {
            return _dict[key].Item1;
        }

        private void Set(TKey key, TValue value)
        {
            _dict[key] = new Tuple<TValue, DateTime>(value, DateTime.Now);
        }

        #endregion
    }
}
