function ProductCard({ product, onAddToCart }) {
    return (
        <div className="product-card">
            <h3>{product.name}</h3>

            <p>{product.description}</p>

            <p className="product-availability">
                {product.isAvailable ? 'Na stanju' : 'Nije dostupno'}
            </p>

            <div className="product-footer">
                <span className="product-price">
                    {product.price} RSD
                </span>

                <button
                    disabled={!product.isAvailable}
                    onClick={() => onAddToCart(product)}
                >
                    Dodaj u korpu
                </button>
            </div>
        </div>
    )
}

export default ProductCard