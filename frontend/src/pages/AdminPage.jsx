import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import adminApi from '../api/adminApi';

export default function AdminPage() {
  const [users, setUsers] = useState([]);
  const [plans, setPlans] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    let ignore = false;
    Promise.allSettled([adminApi.getUsers(), adminApi.getAllTravelPlans()]).then(([u, p]) => {
      if (ignore) return;
      if (u.status === 'fulfilled') setUsers(u.value);
      if (p.status === 'fulfilled') setPlans(p.value);
      if (u.status === 'rejected' || p.status === 'rejected') {
        setError('Neki podaci nisu ucitani - proveri da li backend rute postoje.');
      }
    });
    return () => { ignore = true; };
  }, []);

  const handleDeleteUser = async (id) => {
    await adminApi.deleteUser(id);
    setUsers(await adminApi.getUsers());
  };

  return (
    <div className="container">
      <h1>Admin panel</h1>
      {error && <p className="form-error">{error}</p>}

      <section className="detail-section">
        <h3>Korisnici</h3>
        <ul className="travel-list">
          {users.map((u) => (
            <li key={u.id}>
              <span>{u.name} - {u.email} ({u.role})</span>
              <button onClick={() => handleDeleteUser(u.id)}>Obrisi</button>
            </li>
          ))}
        </ul>
      </section>

      <section className="detail-section">
        <h3>Sva putovanja (svi korisnici)</h3>
        <ul className="travel-list">
          {plans.map((p) => (
            <li key={p.id}>
              <span><strong>{p.name}</strong> - {p.startDate?.slice(0, 10)} do {p.endDate?.slice(0, 10)} (korisnik #{p.userId})</span>
                  <Link to={`/travels/${p.id}`} className="btn-secondary btn-link" style={{ color: 'var(--gray-700)' }}>Otvori</Link>
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}