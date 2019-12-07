using System;
using SeekAndDestroy.Core.DataAccess;

namespace SeekAndDestroy.Core.Game
{
    public class Ticker
    {
        private IUserRepository _userRepository;
        public Ticker(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void DoTick()
        {
            Console.WriteLine("!");
            _userRepository.IncrementAllUserCrystals();
        }
    }
}