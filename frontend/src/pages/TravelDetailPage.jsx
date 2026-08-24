import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import travelApi from '../api/travelApi';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

const EXPENSE_CATEGORIES = ['TRANSPORT', 'ACCOMMODATION', 'FOOD', 'TICKETS', 'SHOPPING', 'OTHER'];
const ACTIVITY_STATUSES = ['PLANNED', 'RESERVED', 'COMPLETED', 'CANCELLED'];

function ActivityCalendar({ travel, activities }) {
    if (!travel) return null;
    const days = [];
    const start = new Date(travel.startDate);
    const end = new Date(travel.endDate);
    for (let d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) days.push(new Date(d));

    return (
        <div className="calendar-grid">
            {days.map((day) => {
                const dayStr = day.toISOString().slice(0, 10);
                const dayActivities = activities.filter((a) => a.date?.slice(0, 10) === dayStr);
                return (
                    <div key={dayStr} className="calendar-day">
                        <div className="calendar-day-header">{day.toLocaleDateString('sr-RS', { day: 'numeric', month: 'short' })}</div>
                        {dayActivities.length === 0 && <p className="calendar-empty">-</p>}
                        {dayActivities.map((a) => (
                            <div key={a.id} className="calendar-activity">{a.time ? `${a.time} ` : ''}{a.name}</div>
                        ))}
                    </div>
                );
            })}
        </div>
    );
}

export default function TravelDetailPage() {
    const { id } = useParams();
    const [tab, setTab] = useState('overview');
    const [travel, setTravel] = useState(null);
    const [destinations, setDestinations] = useState([]);
    const [activities, setActivities] = useState([]);
    const [expenses, setExpenses] = useState([]);
    const [checklist, setChecklist] = useState([]);
    const [budget, setBudget] = useState(null);
    const [shareLink, setShareLink] = useState('');
    const [qrCode, setQrCode] = useState('');

    const [editingTravel, setEditingTravel] = useState(false);
    const [travelForm, setTravelForm] = useState(null);
    const [editingDestId, setEditingDestId] = useState(null);
    const [editingActId, setEditingActId] = useState(null);
    const [editingExpId, setEditingExpId] = useState(null);

    const loadAll = async () => {
        const [t, d, a, e, c, b] = await Promise.all([
            travelApi.getById(id), travelApi.getDestinations(id), travelApi.getActivities(id),
            travelApi.getExpenses(id), travelApi.getChecklist(id), travelApi.getBudgetSummary(id),
        ]);
        setTravel(t); setTravelForm(t); setDestinations(d); setActivities(a); setExpenses(e); setChecklist(c); setBudget(b);
    };

    useEffect(() => {
        let ignore = false;
        (async () => {
            const [t, d, a, e, c, b] = await Promise.all([
                travelApi.getById(id), travelApi.getDestinations(id), travelApi.getActivities(id),
                travelApi.getExpenses(id), travelApi.getChecklist(id), travelApi.getBudgetSummary(id),
            ]);
            if (!ignore) { setTravel(t); setTravelForm(t); setDestinations(d); setActivities(a); setExpenses(e); setChecklist(c); setBudget(b); }
        })();
        return () => { ignore = true; };
    }, [id]);

    const createShare = async (accessType) => {
        const result = await travelApi.createShareLink(id, accessType);
        setShareLink(result.shareUrl);
        setQrCode(result.qrCode);
    };

    const exportPdf = () => {
        const doc = new jsPDF();
        doc.text(travel?.name || 'Plan putovanja', 14, 15);
        doc.setFontSize(10);
        doc.text(`${travel?.startDate?.slice(0, 10)} do ${travel?.endDate?.slice(0, 10)}`, 14, 22);
        autoTable(doc, { startY: 28, head: [['Destinacija', 'Lokacija']], body: destinations.map((d) => [d.name, d.location]) });
        autoTable(doc, { startY: doc.lastAutoTable.finalY + 8, head: [['Aktivnost', 'Datum', 'Status']], body: activities.map((a) => [a.name, a.date?.slice(0, 10), a.status]) });
        autoTable(doc, { startY: doc.lastAutoTable.finalY + 8, head: [['Trosak', 'Kategorija', 'Iznos']], body: expenses.map((ex) => [ex.name, ex.category, `${ex.amount} RSD`]) });
        autoTable(doc, { startY: doc.lastAutoTable.finalY + 8, head: [['Checklist stavka', 'Zavrseno']], body: checklist.map((c) => [c.name, c.isCompleted ? 'Da' : 'Ne']) });
        if (budget) doc.text(`Preostali budzet: ${budget.remainingBudget} RSD`, 14, doc.lastAutoTable.finalY + 10);
        doc.save(`putovanje-${id}.pdf`);
    };

    const saveTravel = async (e) => {
        e.preventDefault();
        await travelApi.update(id, { ...travelForm, budget: Number(travelForm.budget) });
        setEditingTravel(false);
        loadAll();
    };

    const addDestination = async (e) => {
        e.preventDefault();
        const f = e.target;
        await travelApi.addDestination(id, { name: f.name.value, location: f.location.value, arrivalDate: f.arrivalDate.value, departureDate: f.departureDate.value, description: f.description.value });
        f.reset(); loadAll();
    };

    const saveDestination = async (e, destId) => {
        e.preventDefault();
        const f = e.target;
        await travelApi.updateDestination(destId, { name: f.name.value, location: f.location.value, arrivalDate: f.arrivalDate.value, departureDate: f.departureDate.value, description: f.description.value });
        setEditingDestId(null); loadAll();
    };

    const addActivity = async (e) => {
        e.preventDefault();
        const f = e.target;
        await travelApi.addActivity(id, { name: f.name.value, date: f.date.value, time: f.time.value, location: f.location.value, description: f.description.value, estimatedCost: Number(f.estimatedCost.value) || 0, status: f.status.value });
        f.reset(); loadAll();
    };

    const saveActivity = async (e, actId) => {
        e.preventDefault();
        const f = e.target;
        await travelApi.updateActivity(actId, { name: f.name.value, date: f.date.value, time: f.time.value, location: f.location.value, description: f.description.value, estimatedCost: Number(f.estimatedCost.value) || 0, status: f.status.value });
        setEditingActId(null); loadAll();
    };

    const addExpense = async (e) => {
        e.preventDefault();
        const f = e.target;
        await travelApi.addExpense(id, { name: f.name.value, category: f.category.value, amount: Number(f.amount.value), date: f.date.value, description: f.description.value });
        f.reset(); loadAll();
    };

    const saveExpense = async (e, expId) => {
        e.preventDefault();
        const f = e.target;
        await travelApi.updateExpense(expId, { name: f.name.value, category: f.category.value, amount: Number(f.amount.value), date: f.date.value, description: f.description.value });
        setEditingExpId(null); loadAll();
    };

    const addChecklist = async (e) => {
        e.preventDefault();
        const name = e.target.itemName.value;
        if (!name) return;
        await travelApi.addChecklistItem(id, name);
        e.target.reset(); loadAll();
    };

    if (!travel) return <div className="container"><p>Ucitavanje...</p></div>;

    const tabs = [
        { key: 'overview', label: 'Pregled' },
        { key: 'destinations', label: `Destinacije (${destinations.length})` },
        { key: 'activities', label: `Aktivnosti (${activities.length})` },
        { key: 'expenses', label: `Troskovi (${expenses.length})` },
        { key: 'checklist', label: `Checklist (${checklist.length})` },
        { key: 'share', label: 'Deljenje' },
    ];

    return (
        <div className="container">
            <Link to="/travels">&larr; Nazad na listu</Link>
            <header className="app-header">
                <h1>{travel.name}</h1>
                <div className="btn-group">
                    <button onClick={() => setEditingTravel((v) => !v)} className="btn-secondary">
                        {editingTravel ? 'Otkazi' : 'Izmeni'}
                    </button>
                    <button onClick={exportPdf}>Izvezi PDF</button>
                </div>
            </header>

            {!editingTravel && (
                <p>{travel.startDate?.slice(0, 10)} do {travel.endDate?.slice(0, 10)} | Budzet: {travel.budget} RSD</p>
            )}

            {editingTravel && travelForm && (
                <form onSubmit={saveTravel} className="edit-form">
                    <label>Naziv <input value={travelForm.name} onChange={(e) => setTravelForm({ ...travelForm, name: e.target.value })} /></label>
                    <label>Opis <textarea value={travelForm.description || ''} onChange={(e) => setTravelForm({ ...travelForm, description: e.target.value })} /></label>
                    <label>Pocetak <input type="date" value={travelForm.startDate?.slice(0, 10)} onChange={(e) => setTravelForm({ ...travelForm, startDate: e.target.value })} /></label>
                    <label>Kraj <input type="date" value={travelForm.endDate?.slice(0, 10)} onChange={(e) => setTravelForm({ ...travelForm, endDate: e.target.value })} /></label>
                    <label>Budzet <input type="number" value={travelForm.budget} onChange={(e) => setTravelForm({ ...travelForm, budget: e.target.value })} min="0" /></label>
                    <label>Napomene <textarea value={travelForm.notes || ''} onChange={(e) => setTravelForm({ ...travelForm, notes: e.target.value })} /></label>
                    <button type="submit">Sacuvaj</button>
                </form>
            )}

            <div className="tab-bar">
                {tabs.map((t) => (
                    <button key={t.key} className={tab === t.key ? 'tab active' : 'tab'} onClick={() => setTab(t.key)}>{t.label}</button>
                ))}
            </div>

            {tab === 'overview' && budget && (
                <section className="detail-section">
                    <h3>Budzet</h3>
                    <p>Ukupno: {budget.totalBudget} RSD | Potroseno: {budget.totalSpent} RSD | <strong>Preostalo: {budget.remainingBudget} RSD</strong></p>
                    {budget.byCategory?.length > 0 && (
                        <ul>{budget.byCategory.map((c) => <li key={c.category}>{c.category}: {c.total} RSD</li>)}</ul>
                    )}
                </section>
            )}

            {tab === 'destinations' && (
                <section className="detail-section">
                    <h3>Destinacije</h3>
                    <ul className="item-list">
                        {destinations.map((d) => (
                            <li key={d.id}>
                                {editingDestId === d.id ? (
                                    <form onSubmit={(e) => saveDestination(e, d.id)} className="edit-form">
                                        <input name="name" defaultValue={d.name} required />
                                        <input name="location" defaultValue={d.location} required />
                                        <label>Dolazak <input type="date" name="arrivalDate" defaultValue={d.arrivalDate?.slice(0, 10)} required /></label>
                                        <label>Odlazak <input type="date" name="departureDate" defaultValue={d.departureDate?.slice(0, 10)} required /></label>
                                        <input name="description" defaultValue={d.description} placeholder="Opis" />
                                        <div className="btn-group">
                                            <button type="submit">Sacuvaj</button>
                                            <button type="button" className="btn-secondary" onClick={() => setEditingDestId(null)}>Otkazi</button>
                                        </div>
                                    </form>
                                ) : (
                                    <>
                                        <span><strong>{d.name}</strong> - {d.location} ({d.arrivalDate?.slice(0, 10)} do {d.departureDate?.slice(0, 10)})</span>
                                        <div className="btn-group">
                                            <button className="btn-secondary" onClick={() => setEditingDestId(d.id)}>Izmeni</button>
                                            <button className="btn-danger" onClick={async () => { await travelApi.deleteDestination(d.id); loadAll(); }}>Obrisi</button>
                                        </div>
                                    </>
                                )}
                            </li>
                        ))}
                    </ul>
                    <form onSubmit={addDestination} className="inline-form">
                        <input name="name" placeholder="Naziv" required />
                        <input name="location" placeholder="Lokacija" required />
                        <label>Dolazak <input type="date" name="arrivalDate" required /></label>
                        <label>Odlazak <input type="date" name="departureDate" required /></label>
                        <input name="description" placeholder="Opis" />
                        <button type="submit">Dodaj destinaciju</button>
                    </form>
                </section>
            )}

            {tab === 'activities' && (
                <section className="detail-section">
                    <h3>Kalendar aktivnosti</h3>
                    <ActivityCalendar travel={travel} activities={activities} />

                    <h3>Sve aktivnosti</h3>
                    <ul className="item-list">
                        {activities.map((a) => (
                            <li key={a.id}>
                                {editingActId === a.id ? (
                                    <form onSubmit={(e) => saveActivity(e, a.id)} className="edit-form">
                                        <input name="name" defaultValue={a.name} required />
                                        <label>Datum <input type="date" name="date" defaultValue={a.date?.slice(0, 10)} required /></label>
                                        <input name="time" defaultValue={a.time} placeholder="Vreme" />
                                        <input name="location" defaultValue={a.location} required />
                                        <input name="estimatedCost" type="number" defaultValue={a.estimatedCost} min="0" />
                                        <select name="status" defaultValue={a.status}>
                                            {ACTIVITY_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                                        </select>
                                        <input name="description" defaultValue={a.description} placeholder="Opis" />
                                        <div className="btn-group">
                                            <button type="submit">Sacuvaj</button>
                                            <button type="button" className="btn-secondary" onClick={() => setEditingActId(null)}>Otkazi</button>
                                        </div>
                                    </form>
                                ) : (
                                    <>
                                        <span><strong>{a.name}</strong> - {a.date?.slice(0, 10)} {a.time} <span className={`badge badge-${a.status.toLowerCase()}`}>{a.status}</span></span>
                                        <div className="btn-group">
                                            <button className="btn-secondary" onClick={() => setEditingActId(a.id)}>Izmeni</button>
                                            <button className="btn-danger" onClick={async () => { await travelApi.deleteActivity(a.id); loadAll(); }}>Obrisi</button>
                                        </div>
                                    </>
                                )}
                            </li>
                        ))}
                    </ul>
                    <form onSubmit={addActivity} className="inline-form">
                        <input name="name" placeholder="Naziv aktivnosti" required />
                        <label>Datum <input type="date" name="date" required /></label>
                        <input name="time" placeholder="Vreme (npr. 10:00)" />
                        <input name="location" placeholder="Lokacija" required />
                        <input name="estimatedCost" type="number" placeholder="Trosak" min="0" />
                        <select name="status" defaultValue="PLANNED">
                            {ACTIVITY_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                        </select>
                        <input name="description" placeholder="Opis" />
                        <button type="submit">Dodaj aktivnost</button>
                    </form>
                </section>
            )}

            {tab === 'expenses' && (
                <section className="detail-section">
                    <h3>Troskovi</h3>
                    <ul className="item-list">
                        {expenses.map((ex) => (
                            <li key={ex.id}>
                                {editingExpId === ex.id ? (
                                    <form onSubmit={(e) => saveExpense(e, ex.id)} className="edit-form">
                                        <input name="name" defaultValue={ex.name} required />
                                        <select name="category" defaultValue={ex.category}>
                                            {EXPENSE_CATEGORIES.map((c) => <option key={c} value={c}>{c}</option>)}
                                        </select>
                                        <input name="amount" type="number" defaultValue={ex.amount} min="0" required />
                                        <label>Datum <input type="date" name="date" defaultValue={ex.date?.slice(0, 10)} required /></label>
                                        <input name="description" defaultValue={ex.description} placeholder="Opis" />
                                        <div className="btn-group">
                                            <button type="submit">Sacuvaj</button>
                                            <button type="button" className="btn-secondary" onClick={() => setEditingExpId(null)}>Otkazi</button>
                                        </div>
                                    </form>
                                ) : (
                                    <>
                                        <span><strong>{ex.name}</strong> - {ex.amount} RSD <span className="badge badge-category">{ex.category}</span> {ex.date?.slice(0, 10)}</span>
                                        <div className="btn-group">
                                            <button className="btn-secondary" onClick={() => setEditingExpId(ex.id)}>Izmeni</button>
                                            <button className="btn-danger" onClick={async () => { await travelApi.deleteExpense(ex.id); loadAll(); }}>Obrisi</button>
                                        </div>
                                    </>
                                )}
                            </li>
                        ))}
                    </ul>
                    <form onSubmit={addExpense} className="inline-form">
                        <input name="name" placeholder="Naziv troska" required />
                        <select name="category" defaultValue="OTHER">
                            {EXPENSE_CATEGORIES.map((c) => <option key={c} value={c}>{c}</option>)}
                        </select>
                        <input name="amount" type="number" placeholder="Iznos" min="0" required />
                        <label>Datum <input type="date" name="date" required /></label>
                        <input name="description" placeholder="Opis" />
                        <button type="submit">Dodaj trosak</button>
                    </form>
                </section>
            )}

            {tab === 'checklist' && (
                <section className="detail-section">
                    <h3>Checklist / packing lista</h3>
                    <form onSubmit={addChecklist} className="inline-form">
                        <input name="itemName" placeholder="Nova stavka" required />
                        <button type="submit">Dodaj</button>
                    </form>
                    <ul className="item-list">
                        {checklist.map((c) => (
                            <li key={c.id}>
                                <label className="checklist-label">
                                    <input type="checkbox" checked={c.isCompleted} onChange={async () => { await travelApi.toggleChecklistItem(c.id); loadAll(); }} />
                                    {' '}{c.name}
                                </label>
                                <button className="btn-danger" onClick={async () => { await travelApi.deleteChecklistItem(c.id); loadAll(); }}>Obrisi</button>
                            </li>
                        ))}
                    </ul>
                </section>
            )}

            {tab === 'share' && (
                <section className="detail-section">
                    <h3>Deljenje</h3>
                    <div className="btn-group">
                        <button onClick={() => createShare('VIEW')}>Link za pregled</button>
                        <button onClick={() => createShare('EDIT')} className="btn-secondary">Link za izmenu</button>
                    </div>
                    {shareLink && <p className="share-link">{shareLink}</p>}
                    {qrCode && <img src={`data:image/png;base64,${qrCode}`} alt="QR kod" width="150" />}
                </section>
            )}
        </div>
    );
}