import { useState } from 'react';
import { createEmptyTravel } from '../models/Travel';

export default function TravelForm({ onCreate }) {
    const [formData, setFormData] = useState(createEmptyTravel());
    const [error, setError] = useState('');

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (formData.name.trim().length < 2) {
            setError('Naziv putovanja mora imati bar 2 karaktera.');
            return;
        }
        if (new Date(formData.endDate) < new Date(formData.startDate)) {
            setError('Krajnji datum ne moze biti pre pocetnog datuma.');
            return;
        }
        if (Number(formData.budget) < 0) {
            setError('Budzet ne moze biti negativan.');
            return;
        }

        try {
            await onCreate({ ...formData, budget: Number(formData.budget) });
            setFormData(createEmptyTravel());
        } catch (err) {
            setError(err.response?.data?.error || 'Greska prilikom kreiranja putovanja.');
        }
    };

    return (
        <form onSubmit={handleSubmit} className="travel-form">
            <h3>Novo putovanje</h3>
            {error && <p className="form-error">{error}</p>}
            <input
                type="text"
                name="name"
                placeholder="Naziv putovanja"
                value={formData.name}
                onChange={handleChange}
                required
            />
            <textarea
                name="description"
                placeholder="Opis"
                value={formData.description}
                onChange={handleChange}
            />
            <label>
                Pocetak:
                <input type="date" name="startDate" value={formData.startDate} onChange={handleChange} required />
            </label>
            <label>
                Kraj:
                <input type="date" name="endDate" value={formData.endDate} onChange={handleChange} required />
            </label>
            <input
                type="number"
                name="budget"
                placeholder="Budzet"
                value={formData.budget}
                onChange={handleChange}
                min="0"
                required
            />
            <textarea
                name="notes"
                placeholder="Napomene"
                value={formData.notes}
                onChange={handleChange}
            />
            <button type="submit">Kreiraj</button>
        </form>
    );
}