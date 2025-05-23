using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class CustomerReviewService : ICustomerReviewService
{
    private readonly ICustomerReviewRepository _repository;
    private readonly IOrderRepository _orderRepository;

    public CustomerReviewService(ICustomerReviewRepository repository,
        IOrderRepository orderRepository
    )
    {
        _repository = repository;
        _orderRepository = orderRepository;
    }

    public async Task AddReviewAsync(CustomerReviewViewModel model)
    {
        var avgRating = (model.FoodRating + model.ServiceRating + model.AmbienceRating) / 3.0;

        var order = await _orderRepository.GetByIdAsync(model.OrderId) ?? throw new Exception("Order not found.");
        order.Rating = (decimal?)avgRating;
        await _orderRepository.UpdateAsync(order);

        var review = new Customerreview
        {
            Orderid = model.OrderId,
            Foodrating = model.FoodRating,
            Servicerating = model.ServiceRating,
            Ambiencerating = model.AmbienceRating,
            Avgrating = (float)Math.Round(avgRating, 2),
            Comments = model.Comment,
        };

        await _repository.AddCustomerReviewAsync(review);
    }
}
