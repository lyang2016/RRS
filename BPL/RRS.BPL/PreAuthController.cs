using System;
using RRS.BEL;

namespace RRS.BPL
{
    public class PreAuthController
    {
        private readonly IPreAuthProcessor _processor;
        private readonly AuthRequest _request;

        public PreAuthController(IPreAuthProcessor processor, AuthRequest request)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            _processor = processor;

            if (request == null)
                throw  new ArgumentNullException("request");

            _request = request;
        }

        public AuthResponse ProcessPreAuth()
        {
            return _processor.ProcessPreAuth(_request);
        }
    }
}
