// <copyright file="UnitOfWork.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data;

using ELearning.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

public sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? transaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this.transaction is not null)
        {
            return;
        }

        this.transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this.transaction is null)
        {
            return;
        }

        await this.transaction.CommitAsync(cancellationToken);
        await this.transaction.DisposeAsync();
        this.transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this.transaction is null)
        {
            return;
        }

        await this.transaction.RollbackAsync(cancellationToken);
        await this.transaction.DisposeAsync();
        this.transaction = null;
    }
}
