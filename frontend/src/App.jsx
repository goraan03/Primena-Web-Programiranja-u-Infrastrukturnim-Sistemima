import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import TravelsPage from './pages/TravelsPage';
import TravelDetailPage from './pages/TravelDetailPage';
import AdminPage from './pages/AdminPage';
import SharePage from './pages/SharePage';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/share/:token" element={<SharePage />} />
          <Route path="/travels" element={<ProtectedRoute><TravelsPage /></ProtectedRoute>} />
          <Route path="/travels/:id" element={<ProtectedRoute><TravelDetailPage /></ProtectedRoute>} />
          <Route path="/admin" element={<ProtectedRoute><AdminPage /></ProtectedRoute>} />
          <Route path="*" element={<Navigate to="/travels" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;