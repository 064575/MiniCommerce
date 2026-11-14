

import { useEffect, useRef, useState } from 'react'
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
    const [showLoginPassword, setShowLoginPassword] = useState(false)

    const [orders, setOrders] = useState([])
    const [ordersLoading, setOrdersLoading] = useState(false)
    const [ordersError, setOrdersError] = useState('')

    const [showOrders, setShowOrders] = useState(false)

    const ordersAbortController = useRef(null)

    const [registerFirstName, setRegisterFirstName] = useState('')
    const [registerLastName, setRegisterLastName] = useState('')
    const [registerEmail, setRegisterEmail] = useState('')
    const [registerPassword, setRegisterPassword] = useState('')
    const [registerConfirmPassword, setRegisterConfirmPassword] = useState('')
    const [showRegisterPassword, setShowRegisterPassword] = useState(false)
    const [showRegister, setShowRegister] = useState(false)
    const [showConfirmPassword, setShowConfirmPassword] = useState(false)

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

    async function register() {

        if (!registerFirstName.trim()) {
            alert("Ime je obavezno.");
            return;
        }

        if (!registerLastName.trim()) {
            alert("Prezime je obavezno.");
            return;
        }

        if (!registerEmail.trim()) {
            alert("Email je obavezan.");
            return;
        }

        if (registerPassword.length < 8) {
            alert("Lozinka mora imati najmanje 8 karaktera.");
            return;
        }

        if (registerPassword !== registerConfirmPassword) {
            alert("Lozinke se ne poklapaju.");
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/api/users`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    firstName: registerFirstName.trim(),
                    lastName: registerLastName.trim(),
                    email: registerEmail.trim(),
                    password: registerPassword
                })
            })

            if (!response.ok) {
                if (response.status === 409) {
                    throw new Error('Korisnik sa ovom email adresom već postoji.')
                }

                throw new Error('Registracija nije uspela.')
            }

            alert('Registracija uspešna! Sada možeš da se prijaviš.')

            setLoginEmail(registerEmail.trim())
            setLoginPassword('')

            setRegisterFirstName('')
            setRegisterLastName('')
            setRegisterEmail('')
            setRegisterPassword('')
            setRegisterConfirmPassword('')
            setShowRegister(false)

        } catch (err) {
            console.error(err)
            alert(err.message)
        }
    }

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
                firstName: data.firstName,
                lastName: data.lastName,
                email: data.email
            })

            setLoginPassword('')

        } catch (err) {
            console.error(err)
            alert(err.message)
        }
    }

    function logout() {

        ordersAbortController.current?.abort()
        ordersAbortController.current = null

        setToken('')
        setCurrentUser(null)
        setLoginEmail('')
        setLoginPassword('')
        setOrderMessage('')

        setOrders([])
        setOrdersLoading(false)
        setOrdersError('')

        setShowOrders(false)

    }

    async function loadOrders() {
        if (!token) {
            return
        }

        ordersAbortController.current?.abort()

        const controller = new AbortController()
        ordersAbortController.current = controller

        setOrdersLoading(true)
        setOrdersError('')

        try {
            const response = await fetch(
                `${API_BASE_URL}/api/orders`,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    },
                    signal: controller.signal
                }
            )

            if (!response.ok) {
                throw new Error(
                    'Greška pri učitavanju porudžbina.'
                )
            }

            const data = await response.json()

            if (!controller.signal.aborted) {
                setOrders(data)
            }

        } catch (err) {
            if (!controller.signal.aborted) {
                console.error(err)
                setOrdersError(err.message)
            }

        } finally {
            if (ordersAbortController.current === controller) {
                ordersAbortController.current = null
                setOrdersLoading(false)
            }
        }
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

            if (showOrders) {
                await loadOrders()
            }

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
                    <p>Dobrodošao, {currentUser.firstName} {currentUser.lastName}!</p>

                    <button
                        type="button"
                        onClick={() => {
                            if (showOrders) {
                                setShowOrders(false)
                            } else {
                                setShowOrders(true)
                                loadOrders()
                            }
                        }}
                    >
                        {showOrders ? 'Sakrij porudžbine' : 'Moje porudžbine'}
                    </button>

                    <button type="button" onClick={logout}>
                        Odjavi se
                    </button>
                </div>

            ) : (
                <div>
                    {showRegister ? (
                        <div>
                            <h2>Registracija</h2>

                            <form onSubmit={async (event) => {
                                event.preventDefault()
                                await register()
                            }}>

                                    <div>
                                        <label htmlFor="register-first-name">Ime:</label>
                                        <input
                                            id="register-first-name"
                                            type="text"
                                            value={registerFirstName}
                                            onChange={(event) =>
                                                setRegisterFirstName(event.target.value)
                                            }
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label htmlFor="register-last-name">Prezime:</label>
                                        <input
                                            id="register-last-name"
                                            type="text"
                                            value={registerLastName}
                                            onChange={(event) =>
                                                setRegisterLastName(event.target.value)
                                            }
                                            required
                                        />
                                    </div>


                                <div>
                                    <label htmlFor="register-email">Email:</label>
                                    <input
                                        id="register-email"
                                        type="email"
                                        value={registerEmail}
                                        onChange={(event) =>
                                            setRegisterEmail(event.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div>
                                    <label htmlFor="register-password">Lozinka:</label>
                                        <input
                                            id="register-password"
                                            type={showRegisterPassword ? "text" : "password"}
                                            value={registerPassword}
                                            onChange={(event) =>
                                                setRegisterPassword(event.target.value)
                                            }
                                            required
                                        />

                                        <button
                                            type="button"
                                            onClick={() => setShowRegisterPassword(!showRegisterPassword)}
                                        >
                                            {showRegisterPassword ? "Sakrij" : "Prikaži"}
                                        </button>
                                    </div>

                                    <div>
                                        <label htmlFor="register-confirm-password">
                                            Potvrdi lozinku:
                                        </label>
                                        <input
                                            id="register-confirm-password"
                                            type={showConfirmPassword ? "text" : "password"}
                                            value={registerConfirmPassword}
                                            onChange={(event) =>
                                                setRegisterConfirmPassword(event.target.value)
                                            }
                                            required
                                        />

                                        <button
                                            type="button"
                                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                        >
                                            {showConfirmPassword ? "Sakrij" : "Prikaži"}
                                        </button>
                                    </div>

                                <button type="submit">
                                    Registruj se
                                </button>
                            </form>

                                <button
                                    type="button"
                                    onClick={() => {
                                        setShowRegister(false)
                                        setShowRegisterPassword(false)
                                        setShowConfirmPassword(false)
                                    }}
                                >
                                    Nazad na prijavu
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
                                                type={showLoginPassword ? "text" : "password"}
                                                value={loginPassword}
                                                onChange={(event) =>
                                                    setLoginPassword(event.target.value)
                                                }
                                                required
                                            />

                                            <button
                                                type="button"
                                                onClick={() => setShowLoginPassword(!showLoginPassword)}
                                            >
                                                {showLoginPassword ? "Sakrij" : "Prikaži"}
                                            </button>
                                </div>

                                <button type="submit">
                                    Prijavi se
                                </button>
                            </form>

                                    <button
                                        type="button"
                                        onClick={() => {
                                            setShowRegister(true)
                                            setShowLoginPassword(false)
                                        }}
                                    >
                                        Napravi nalog
                                    </button>
                        </div>
                    )}
                </div>
            )}


            {orderMessage && (
                <p className="order-message">
                    {orderMessage}
                </p>
            )}

            {currentUser && showOrders && (
                <section>
                    <h2>Moje porudžbine</h2>

                    {ordersLoading && <p>Učitavanje porudžbina...</p>}

                    {ordersError && <p>{ordersError}</p>}

                    {!ordersLoading && !ordersError && orders.length === 0 && (
                        <p>Nema učitanih porudžbina.</p>
                    )}

                    {orders.map(order => (
                        <div key={order.id}>
                            <p>Broj porudžbine: {order.id}</p>
                            <p>Status: {order.status}</p>
                            <p>Ukupno: {order.totalPrice.toFixed(2)} RSD</p>
                            <p>
                                Datum: {new Date(order.createdAt).toLocaleString('sr-RS')}
                            </p>

                            <h3>Stavke porudžbine</h3>

                            <ul>
                                {order.items.map(item => (
                                    <li key={item.productId}>
                                        Proizvod: {item.productName || products.find(p => p.id === item.productId)?.name || item.productId}
                                        {' | '}
                                        Količina: {item.quantity}
                                        {' | '}
                                        Jedinična cena: {item.unitPrice.toFixed(2)} RSD
                                    </li>
                                ))}
                            </ul>

                            <hr />
                        </div>
                    ))}
                </section>
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