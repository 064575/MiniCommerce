import { useEffect, useState } from 'react'
import ProductCard from './components/ProductCard'
import './App.css'
import Cart from './components/Cart'

const API_BASE_URL =
    import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

function App() {
    const [products, setProducts] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')
    const [cart, setCart] = useState([])
    const [orderMessage, setOrderMessage] = useState('')
    const [token, setToken] = useState('')
    const [currentUser, setCurrentUser] = useState(null)
    const [loginEmail, setLoginEmail] = useState('')
    const [loginPassword, setLoginPassword] = useState('')

    useEffect(() => {
        async function loadProducts() {
            try {
                const response = await fetch(`${API_BASE_URL}/api/catalog`)

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

    async function login() {
        try {
            const response = await fetch(
                `${API_BASE_URL}/api/users/login`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        email: loginEmail,
                        password: loginPassword
                    })
                }
            )

            if (!response.ok) {
                throw new Error('Pogrešan email ili lozinka.')
            }

            const data = await response.json()

            setToken(data.token)

            setCurrentUser({
                userId: data.userId,
                name: data.name,
                email: data.email
            })

            setLoginPassword('')

        } catch (err) {
            console.error(err)
            alert(err.message)
        }
    }

    function logout() {
        setToken('')
        setCurrentUser(null)
        setLoginEmail('')
        setLoginPassword('')
        setOrderMessage('')
    }

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

        if (!token) {
            setOrderMessage(
                'Morate se prijaviti pre kreiranja porudžbine.'
            )
            return
        }

        const request = {
            items: cart.map(item => ({
                productId: item.id,
                quantity: item.quantity
            }))
        }

        try {
            const response = await fetch(
                `${API_BASE_URL}/api/orders`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    },
                    body: JSON.stringify(request)
                }
            )

            if (!response.ok) {
                if (response.status === 401) {
                    throw new Error(
                        'Prijava nije važeća. Prijavite se ponovo.'
                    )
                }

                throw new Error(
                    'Porudžbina nije uspešno kreirana.'
                )
            }

            const order = await response.json()

            console.log('Kreirana porudžbina:', order)

            setOrderMessage(
                `Porudžbina je uspešno kreirana! Status: ${order.status}. Ukupno: ${order.totalPrice.toFixed(2)} RSD`
            )

            setCart([])
        } catch (error) {
            console.error(error)

            setOrderMessage(`Greška: ${error.message}`)
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

            {currentUser ? (
                <div>
                    <p>Dobrodošao, {currentUser.name}!</p>

                    <button type="button" onClick={logout}>
                        Odjavi se
                    </button>
                </div>
            ) : (
                <div>
                    <h2>Prijava</h2>

                    <form onSubmit={async (event) => {
                        event.preventDefault()
                        await login()
                    }}>
                        <div>
                            <label htmlFor="login-email">Email:</label>
                            <input
                                id="login-email"
                                type="email"
                                value={loginEmail}
                                onChange={(event) =>
                                    setLoginEmail(event.target.value)
                                }
                                required
                            />
                        </div>

                        <div>
                            <label htmlFor="login-password">Lozinka:</label>
                            <input
                                id="login-password"
                                type="password"
                                value={loginPassword}
                                onChange={(event) =>
                                    setLoginPassword(event.target.value)
                                }
                                required
                            />
                        </div>

                        <button type="submit">
                            Prijavi se
                        </button>
                    </form>
                </div>
            )}

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