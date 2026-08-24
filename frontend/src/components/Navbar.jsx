import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const isActive = (path) => location.pathname === path;

  return (
    <nav className="navbar">
      <Link to="/travels" className="navbar-brand">TravelPlanner</Link>
      <div className="navbar-links">
        <Link to="/travels" className={isActive('/travels') ? 'active' : ''}>Moja putovanja</Link>
        {user?.role === 'ADMIN' && (
          <Link to="/admin" className={`navbar-admin-link ${isActive('/admin') ? 'active' : ''}`}>Admin panel</Link>
        )}
        <span className="navbar-divider" />
        <span className="navbar-user">{user?.name}</span>
        <button onClick={handleLogout} className="btn-secondary">Odjavi se</button>
      </div>
    </nav>
  );
}