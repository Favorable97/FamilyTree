using FamilyTree.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTree.Data.Repositories
{
    public class UnitOfWork(FamilyTreeContext context) : IUnitOfWork
    {
        private readonly FamilyTreeContext _context = context;

        public Task BeginTransactionAsync() => _context.BeginTransactionAsync();

        public Task CommitTransactionAsync() => _context.CommitTransactionAsync();

        public Task RollbackTransactionAsync() => _context.RollbackTransactionAsync();
    }
}
