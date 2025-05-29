using FUNewsManagementSystem.Reposirories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface ITagService
    {
        IQueryable<Tag> GetTags();
    }
}
