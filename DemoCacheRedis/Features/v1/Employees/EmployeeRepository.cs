using Bogus;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCacheRedis.Features.v1.Employees
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployees();
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDistributedCache _cache;
        private const string KEY_ALL_EMPLOYEES = "ALL_EMPLOYEES";

        public EmployeeRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        private IEnumerable<Employee> EmployeesFromDatabase()
        {
            var count = 1;

            var list = new Faker<Employee>()
                .RuleFor(p => p.Id, p => count++)
                .RuleFor(p => p.Name, p => p.Person.FullName)
                .RuleFor(p => p.Salary, p => p.Random.Decimal(1000, 10000).ToString("U$ 0.##"))
                .GenerateLazy(100);

            return list;
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            var dataCache = await _cache.GetStringAsync(KEY_ALL_EMPLOYEES);

            if (string.IsNullOrWhiteSpace(dataCache))
            {
                var cacheSettings = new DistributedCacheEntryOptions();
                cacheSettings.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                var employeesFromDatabase = EmployeesFromDatabase();

                var itemsJson = JsonConvert.SerializeObject(employeesFromDatabase);

                await _cache.SetStringAsync(KEY_ALL_EMPLOYEES, itemsJson, cacheSettings);

                return await Task.FromResult(employeesFromDatabase);
            }

            var employeesFromCache = JsonConvert.DeserializeObject<IEnumerable<Employee>>(dataCache);

            return await Task.FromResult(employeesFromCache);
        }
    }
}
