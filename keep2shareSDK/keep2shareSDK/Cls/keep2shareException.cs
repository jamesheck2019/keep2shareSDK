using System;

namespace keep2shareSDK
{
    public class keep2shareException : Exception
    {
        public keep2shareException(string errorMesage, int errorCode) : base(errorMesage) { }

    }


}
