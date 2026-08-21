import { createContext, useContext, useState } from 'react';
import authApi from '../api/authApi';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(() => {
        const stored = localStorage.getItem('user');
        return stored ? JSON.parse(stored) : null;
    });
    const [error, setError] = useState('');

    const login = async (email, password) => {
        setError('');
        try {
            const result = await authApi.login({ email, password });
            localStorage.setItem('token', result.token);
            localStorage.setItem('user', JSON.stringify(result.user));
            setUser(result.user);
            return true;
        } catch (err) {
            setError(err.response?.data?.error || 'Prijava nije uspela.');
            return false;
        }
    };

    const register = async (name, email, password) => {
        setError('');
        try {
            await authApi.register({ name, email, password });
            return true;
        } catch (err) {
            setError(err.response?.data?.error || 'Registracija nije uspela.');
            return false;
        }
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, error, login, register, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}