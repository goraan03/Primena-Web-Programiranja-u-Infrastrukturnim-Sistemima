import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { createEmptyUser } from '../models/Travel';
import { isValidEmail, isValidPassword, isValidName } from '../utils/validators';

export default function RegisterForm() {
    const [formData, setFormData] = useState(createEmptyUser());
    const [localError, setLocalError] = useState('');
    const { register, error } = useAuth();
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLocalError('');

        if (!isValidName(formData.name)) {
            setLocalError('Ime mora imati bar 2 karaktera.');
            return;
        }
        if (!isValidEmail(formData.email)) {
            setLocalError('Unesite ispravnu email adresu (npr. ime@primer.com).');
            return;
        }
        if (!isValidPassword(formData.password)) {
            setLocalError('Lozinka mora imati bar 6 karaktera.');
            return;
        }

        const success = await register(formData.name, formData.email, formData.password);
        if (success) navigate('/login');
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Registracija</h2>
            {(localError || error) && <p className="form-error">{localError || error}</p>}
            <input
                type="text"
                name="name"
                placeholder="Ime"
                value={formData.name}
                onChange={handleChange}
                required
            />
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
            <button type="submit">Registruj se</button>
            <p>
                Vec imas nalog? <Link to="/login">Prijavi se</Link>
            </p>
        </form>
    );
}