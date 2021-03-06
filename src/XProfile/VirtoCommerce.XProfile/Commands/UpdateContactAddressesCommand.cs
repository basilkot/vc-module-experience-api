using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateContactAddressesCommand : ICommand<ContactAggregate>
    {
        public UpdateContactAddressesCommand()
        {

        }

        public UpdateContactAddressesCommand(string contactId, IList<Address> addresses)
        {
            ContactId = contactId;
            Addresses = addresses;
        }

        public string ContactId { get; set; }
        public IList<Address> Addresses { get; set; }
    }
}
