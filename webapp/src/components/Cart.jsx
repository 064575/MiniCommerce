function Cart({
    cart,
    onIncrease,
    onDecrease,
    onRemove,
    onCreateOrder
}) {
    const totalPrice = cart.reduce(
        (total, item) => total + item.price * item.quantity,
        0
    )

    return (
        <div className="cart">
            <h2>Korpa</h2>

            {cart.length === 0 ? (
                <p>Korpa je prazna.</p>
            ) : (
                <div>
                    {cart.map(item => (
                        <div key={item.id} className="cart-item">
                            <span>{item.name}</span>

                            <div>
                                <button onClick={() => onDecrease(item.id)}>
                                    -
                                </button>

                                <span>
                                    {item.quantity} x {item.price} RSD
                                </span>

                                <button onClick={() => onIncrease(item.id)}>
                                    +
                                </button>

                                <button onClick={() => onRemove(item.id)}>
                                    Ukloni
                                </button>
                            </div>
                        </div>
                    ))}

                    <p className="cart-total">
                        Ukupno: {totalPrice.toFixed(2)} RSD
                        </p>

                        <button onClick={onCreateOrder}>
                            Naruči
                        </button>

                </div>
            )}
        </div>
    )
}

export default Cart