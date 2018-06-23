﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubContainer
   {
      Task<IEnumerable<StubModel>> GetStubsAsync();

      Task AddStubAsync(StubModel stub);

      Task<bool> DeleteStubAsync(string stubId);

      Task<StubModel> GetStubAsync(string stubId);

      Task AddRequestResultAsync(RequestResultModel requestResult);

      Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();

      Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId);
   }
}
