import { Link } from 'react-router-dom';

export default function TravelList({ travels, onDelete }) {
    if (travels.length === 0) {
        return <p>Nemas jos kreiranih putovanja.</p>;
    }

    return (
        <ul className="travel-list">
            {travels.map((travel) => (
                <li key={travel.id}>
                    <span>
                        <strong>{travel.name}</strong> - {travel.startDate?.slice(0, 10)} do {travel.endDate?.slice(0, 10)}
                        {' '}(budzet: {travel.budget} RSD) - <Link to={`/travels/${travel.id}`}>Detalji</Link>
                    </span>
                    <button onClick={() => onDelete(travel.id)}>Obrisi</button>
                </li>
            ))}
        </ul>
    );
}