using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTree.Data.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task RollbackTransactionAsync();
        Task CommitTransactionAsync();
    }
}
