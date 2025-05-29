using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Tag> GetTags()
        {
            return _unitOfWork._tagRepository.GetAll();
        }  
    }
}
