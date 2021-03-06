﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineStore.Model;

namespace OnlineStore.DataAccess.Implementation
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _orders = new Dictionary<Guid, Order>();

        public Task CreateAsync(Order order)
        {
            _orders.Add(order.Id, order);
            return Task.FromResult(0);
        }

        public Task UpdateAsync(Order order)
        {
            if (!_orders.ContainsKey(order.Id))
            {
                throw new RepositoryException("Order does not exist");
            }
            _orders[order.Id] = order;
            return Task.FromResult(0);
        }

        public Task<Order> GetAsync(Guid orderId)
        {
            return Task.FromResult(_orders[orderId]);
        }
    }
}
