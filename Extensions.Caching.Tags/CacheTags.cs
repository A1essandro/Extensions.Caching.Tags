using System.Collections.Generic;

namespace Microsoft.Extensions.Caching.Memory
{

    public class CacheTags : List<string>
    {

        public CacheTags(params string[] tags) : base(tags) { }

        public CacheTags(IEnumerable<string> tags) : base(tags) { }

        public static implicit operator CacheTags(string[] data) => new CacheTags(data);

    }


}
