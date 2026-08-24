import { Link } from 'react-router-dom';

export default function TravelList({ travels, onDelete }) {
    if (travels.length === 0) {
        return <p style={{ color: 'var(--gray-500)' }}>Nemas jos kreiranih putovanja.</p>;
    }

    return (
        <ul className="travel-list">
            {travels.map((travel) => (
                <li key={travel.id}>
                    <div className="travel-list-info">
                        <strong>{travel.name}</strong>
                        <span style={{ color: 'var(--gray-500)', fontSize: 13 }}>
                            {travel.startDate?.slice(0, 10)} do {travel.endDate?.slice(0, 10)} - {travel.budget} RSD
                        </span>
                    </div>
                    <div className="travel-list-actions">
                        <Link to={`/travels/${travel.id}`} className="btn-link">Detalji</Link>
                        <button className="btn-danger" onClick={() => onDelete(travel.id)}>Obrisi</button>
                    </div>
                </li>
            ))}
        </ul>
    );
}