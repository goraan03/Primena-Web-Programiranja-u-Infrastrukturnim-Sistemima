import { useEffect, useState } from 'react';
import travelApi from '../api/travelApi';
import TravelForm from '../components/TravelForm';
import TravelList from '../components/TravelList';
import { useAuth } from '../contexts/AuthContext';
import { Link } from 'react-router-dom';

export default function TravelsPage() {
    const [travels, setTravels] = useState([]);
    const [loading, setLoading] = useState(true);
    const { user, logout } = useAuth();

    const loadTravels = async () => {
        setLoading(true);
        try {
            const data = await travelApi.getAll();
            setTravels(data);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        let ignore = false;

        travelApi.getAll().then((data) => {
            if (!ignore) {
                setTravels(data);
                setLoading(false);
            }
        });

        return () => {
            ignore = true;
        };
    }, []);

    const handleCreate = async (data) => {
        await travelApi.create(data);
        await loadTravels();
    };

    const handleDelete = async (id) => {
        await travelApi.delete(id);
        await loadTravels();
    };

    return (

        <div className="container">
            <header className="app-header">
                <h1>Moja putovanja {user ? `- ${user.name}` : ''}</h1>
                <div>
                    {user?.role === 'ADMIN' && <Link to="/admin" style={{ marginRight: 12 }}>Admin panel</Link>}
                    <button onClick={logout}>Odjavi se</button>
                </div>
            </header>

            <TravelForm onCreate={handleCreate} />

            {loading ? <p>Ucitavanje...</p> : <TravelList travels={travels} onDelete={handleDelete} />}
        </div>
    );
}