import { useEffect, useState } from 'react'
import ProductCard from './components/ProductCard'
import './App.css'
import Cart from './components/Cart'

function App() {
    const [products, setProducts] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')
    const [cart, setCart] = useState([])
    const [orderMessage, setOrderMessage] = useState('')

    useEffect(() => {
        async function loadProducts() {
            try {
                const response = await fetch('http://localhost:5000/api/catalog')

                if (!response.ok) {
                    throw new Error('Greška pri učitavanju proizvoda.')
                }

                const data = await response.json()
                setProducts(data)
            } catch (err) {
                setError(err.message)
            } finally {
                setLoading(false)
            }
        }

        loadProducts()
    }, [])

    function addToCart(product) {
        const existingItem = cart.find(
            item => item.id === product.id
        )

        if (existingItem) {
            setCart(
                cart.map(item =>
                    item.id === product.id
                        ? { ...item, quantity: item.quantity + 1 }
                        : item
                )
            )
        } else {
            setCart([
                ...cart,
                {
                    ...product,
                    quantity: 1
                }
            ])
        }
    }

    function increaseQuantity(productId) {
        setCart(
            cart.map(item =>
                item.id === productId
                    ? { ...item, quantity: item.quantity + 1 }
                    : item
            )
        )
    }

    function decreaseQuantity(productId) {
        setCart(
            cart.map(item =>
                item.id === productId && item.quantity > 1
                    ? { ...item, quantity: item.quantity - 1 }
                    : item
            )
        )
    }

    function removeFromCart(productId) {
        setCart(
            cart.filter(item => item.id !== productId)
        )
    }

    async function createOrder() {

        setOrderMessage('')

        const request = {
            userId: '513e2dd0-1146-4944-bd50-4d3287a69fe1',
            items: cart.map(item => ({
                productId: item.id,
                quantity: item.quantity
            }))
        }

        try {
            const response = await fetch(
                'http://localhost:5000/api/orders',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(request)
                }
            )

            if (!response.ok) {
                throw new Error('Porudžbina nije uspešno kreirana.')
            }

            const order = await response.json()

            console.log('Kreirana porudžbina:', order)

            setOrderMessage(
                `Porudžbina je uspešno kreirana! Status: ${order.status}. Ukupno: ${order.totalPrice.toFixed(2)} RSD`
            )

            setCart([])
        } catch (error) {
            console.error(error)

            setOrderMessage(
                `Greška: ${error.message}`
            )
        }
    }


    if (loading) {
        return <p>Učitavanje proizvoda...</p>
    }

    if (error) {
        return <p>{error}</p>
    }

    return (
        <div>
            <h1>MiniCommerce</h1>

            {orderMessage && (
                <p className="order-message">
                    {orderMessage}
                </p>
            )}

            <p className="cart-count">
                Korpa: {cart.length}
            </p>

            <Cart
                cart={cart}
                onIncrease={increaseQuantity}
                onDecrease={decreaseQuantity}
                onRemove={removeFromCart}
                onCreateOrder={createOrder}
            />

            <h2>Proizvodi</h2>

            {products.length === 0 ? (
                <p>Nema proizvoda.</p>
            ) : (
                <div className="products-grid">
                    {products.map(product => (
                        <ProductCard
                            key={product.id}
                            product={product}
                            onAddToCart={addToCart}
                        />
                    ))}
                </div>
            )}
        </div>
    )
}

export default App