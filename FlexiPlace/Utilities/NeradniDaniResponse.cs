using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Utilities
{
    public class NeradniDaniResponse<T>
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public static NeradniDaniResponse<T> GetResult(int code, string msg, T data = default(T))
        {
            return new NeradniDaniResponse<T>
            {
                Code = code,
                Msg = msg,
                Data = data
            };
        }
    }
}
