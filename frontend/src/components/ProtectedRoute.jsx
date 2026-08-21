import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export default function ProtectedRoute({ children }) {
    const { user } = useAuth();
    if (!user && !localStorage.getItem('token')) {
        return <Navigate to="/login" replace />;
    }
    return children;
}