using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.ReadModel.EventHandlers
{
    /// <summary>
    /// Listens order change events
    /// </summary>
    public class OrderEventListener
    {
        private IPersistanceRepository _persistanceRepository;

        public OrderEventListener(IPersistanceRepository persistanceRepository)
        {
            _persistanceRepository = persistanceRepository;
            OrderEvent.OrderChanged += OnOrderStatusChanged;
        }

        void OnOrderStatusChanged(Order order)
        {
            _persistanceRepository.SaveOrUpdate(OrderReadModel.CreateOrderReadModel(order));
        }
    }
}
