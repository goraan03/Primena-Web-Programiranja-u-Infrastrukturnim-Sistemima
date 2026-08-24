import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axiosInstance from '../api/axiosInstance';

export default function SharePage() {
    const { token } = useParams();
    const [data, setData] = useState(null);
    const [form, setForm] = useState(null);
    const [error, setError] = useState('');
    const [saved, setSaved] = useState(false);

    useEffect(() => {
        axiosInstance.get(`/share/${token}`)
            .then((res) => { setData(res.data); setForm(res.data.travel); })
            .catch(() => setError('Link nije vazeci ili je istekao.'));
    }, [token]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleSave = async (e) => {
        e.preventDefault();
        setError(''); setSaved(false);
        try {
            await axiosInstance.put(`/share/${token}`, form);
            setSaved(true);
        } catch (err) {
            setError(err.response?.data?.error || 'Greska prilikom izmene.');
        }
    };

    if (error && !data) return <div className="page-center"><div className="card"><p className="form-error">{error}</p></div></div>;
    if (!data) return <div className="container"><p>Ucitavanje...</p></div>;

    const isEdit = data.accessType === 'EDIT';

    return (
        <div className="container">
            <div className="detail-section">
                <h1>{data.travel.name}</h1>
                <span className={isEdit ? 'badge badge-edit' : 'badge badge-view'}>
                    {isEdit ? 'Pristup: izmena' : 'Pristup: samo pregled'}
                </span>

                {!isEdit && (
                    <>
                        <p>{data.travel.description}</p>
                        <p>{data.travel.startDate?.slice(0, 10)} do {data.travel.endDate?.slice(0, 10)}</p>
                        <p>Budzet: {data.travel.budget} RSD</p>
                        <p>{data.travel.notes}</p>
                    </>
                )}

                {isEdit && form && (
                    <form onSubmit={handleSave} className="edit-form">
                        {saved && <p className="form-success">Izmene sacuvane.</p>}
                        {error && <p className="form-error">{error}</p>}
                        <label>Naziv <input name="name" value={form.name} onChange={handleChange} /></label>
                        <label>Opis <textarea name="description" value={form.description || ''} onChange={handleChange} /></label>
                        <label>Pocetak <input type="date" name="startDate" value={form.startDate?.slice(0, 10)} onChange={handleChange} /></label>
                        <label>Kraj <input type="date" name="endDate" value={form.endDate?.slice(0, 10)} onChange={handleChange} /></label>
                        <label>Budzet <input type="number" name="budget" value={form.budget} onChange={handleChange} min="0" /></label>
                        <label>Napomene <textarea name="notes" value={form.notes || ''} onChange={handleChange} /></label>
                        <button type="submit">Sacuvaj izmene</button>
                    </form>
                )}
            </div>
        </div>
    );
}