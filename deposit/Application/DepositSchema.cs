using Deposit.Application.GraphTypes;
using Deposit.Infrastructure.GraphQl;

namespace Deposit.Application;

internal class DepositSchema : FederatedSchema
{
    public DepositSchema(IServiceProvider provider, DepositQuery depositQuery) : base(provider)
    {
        Query = depositQuery;
    }
}