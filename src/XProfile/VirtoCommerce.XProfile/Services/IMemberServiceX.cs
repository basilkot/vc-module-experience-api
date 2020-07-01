using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Services
{
    public interface IMemberServiceX
    {
        Task<Profile> GetProfileByIdAsync(string userId);
        Task<Contact> CreateContactAsync(Contact contact);
        Task<Profile> UpdateContactAsync(UserUpdateInfo userUpdateInfo);
        Task<IdentityResult> UpdatePhoneNumberAsync(PhoneNumberUpdateInfo updateInfo);
        Task<IdentityResult> RemovePhoneNumberAsync(string userId);
        Task<Contact> UpdateContactAddressesAsync(string contactId, IList<Address> addresses);
        Task DeleteContactAsync(string contactId);

        //IPagedList<Contact> SearchOrganizationContacts(OrganizationContactsSearchCriteria criteria);
        //Task<IPagedList<Contact>> SearchOrganizationContactsAsync(OrganizationContactsSearchCriteria criteria);

        Task<Organization> GetOrganizationByIdAsync(string organizationId);
        Task<Organization> CreateOrganizationAsync(Organization organization);
        Task<Organization> UpdateOrganizationAsync(OrganizationUpdateInfo organizationUpdateInfo);


        //Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language, params string[] vendorIds);
        //Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds);
        //IPagedList<Vendor> SearchVendors(Store store, Language language, string keyword, int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos);
    }
}