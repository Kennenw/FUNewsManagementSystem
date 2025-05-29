
using AutoMapper;
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;
using Microsoft.Extensions.Options;

namespace FUNewsManagementSystem.Services
{
    public class SystemAccountService : ISystemAccountService
    {
        public IUnitOfWork _unitOfWork;
        private readonly AdminAccount _adminAccount;
        private readonly IMapper _mapper;
        public SystemAccountService(IUnitOfWork unitOfWork, IOptions<AdminAccount> adminAccount, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _adminAccount = adminAccount.Value;
        }
        public async Task AddAsync(CreateSystemAccountViewModel entity)
        {
            SystemAccount systemAccount = _mapper.Map<SystemAccount>(entity);
            await _unitOfWork._systemAccountRepository.AddAsync(systemAccount);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            await _unitOfWork._systemAccountRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<SystemAccount> GetAllAsync()
        {
            return _unitOfWork._systemAccountRepository.GetAll();
        }

        public async Task<SystemAccount?> GetByIdAsync(short id)
        {
            return await _unitOfWork._systemAccountRepository.GetByIdAsync(id);
        }

        public async Task<SystemAccount?> LoginAsync(LoginRequestViewModel model)
        {
            if (model.Email == _adminAccount.Email && model.Password == _adminAccount.Password)
            {
                return new SystemAccount { 
                    AccountEmail = model.Email,
                    AccountRole = int.Parse(_adminAccount.Role)
                };
            }
            var user = await _unitOfWork._systemAccountRepository.LoginAsync(model);
            return user;
        }


        public async Task UpdateAsync(short id, UpdateSystemAccountViewModel entity)
        {
            var item = await _unitOfWork._systemAccountRepository.GetByIdAsync(id);
            if (item == null) return;
            _mapper.Map(entity, item);
            await _unitOfWork._systemAccountRepository.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CheckUserAsync(short id)
        {
            var user = await _unitOfWork._systemAccountRepository.GetByIdAsync(id);
            if(user != null)
            {
                return await _unitOfWork._systemAccountRepository.CheckUserAsync(id);
            }
            return true;
        }
    }
}
