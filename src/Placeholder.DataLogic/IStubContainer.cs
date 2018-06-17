﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.DataLogic
{
   public interface IStubContainer
   {
      Task<IEnumerable<StubModel>> GetStubsAsync();
   }
}
