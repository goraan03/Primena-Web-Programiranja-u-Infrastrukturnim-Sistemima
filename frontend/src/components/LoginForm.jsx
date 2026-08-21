import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { isValidEmail } from '../utils/validators';

export default function LoginForm() {
    const [formData, setFormData] = useState({ email: '', password: '' });
    const [localError, setLocalError] = useState('');
    const { login, error } = useAuth();
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLocalError('');

        if (!isValidEmail(formData.email)) {
            setLocalError('Unesite ispravnu email adresu.');
            return;
        }

        const success = await login(formData.email, formData.password);
        if (success) navigate('/travels');
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Prijava</h2>
            {(localError || error) && <p className="form-error">{localError || error}</p>}
            <input
                type="email"
                name="email"
                placeholder="Email"
                value={formData.email}
                onChange={handleChange}
                required
            />
            <input
                type="password"
                name="password"
                placeholder="Lozinka"
                value={formData.password}
                onChange={handleChange}
                required
            />
            <button type="submit">Prijavi se</button>
            <p>
                Nemas nalog? <Link to="/register">Registruj se</Link>
            </p>
        </form>
    );
}