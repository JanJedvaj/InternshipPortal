import { useEffect, useState } from "react";
import {
  deleteInternship,
  updateInternship,
  searchInternships,
} from "../api.js";
import EditInternshipForm from "./EditInternshipForm";

function formatDate(dateString) {
  if (!dateString) return "-";
  const date = new Date(dateString);
  return date.toLocaleDateString("hr-HR");
}

export default function InternshipList() {
  const [internships, setInternships] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [actionError, setActionError] = useState("");

  const [editItem, setEditItem] = useState(null);

 //STAnja za search i Sort
  const [keyword, setKeyword] = useState("");
  const [sortBy, setSortBy] = useState("date");       // "date" | "deadline" | "title"
  const [sortDescending, setSortDescending] = useState(true);

  async function load() {
    setLoading(true);
    setError("");

    try {
      const result = await searchInternships({
        keyword,
        sortBy,
        sortDescending,
        onlyActive: true,
        page: 1,
        pageSize: 50,
      });

      setInternships(result.items || []);
    } catch (err) {
      setError(err.message || "Dogodila se greška prilikom dohvaćanja.");
    } finally {
      setLoading(false);
    }
  }

  // defaultni krijterij
  useEffect(() => {
    load();
    
  }, []);

  async function handleDelete(id) {
    const ok = window.confirm("Jesi li siguran da želiš obrisati ovaj oglas?");
    if (!ok) return;

    setActionError("");

    try {
      await deleteInternship(id);
      await load();
    } catch (err) {
      setActionError(err.message || "Greška prilikom brisanja.");
    }
  }

  async function handleSaveEdit(updated) {
    setActionError("");

    try {
      await updateInternship(updated.id, updated);
      setEditItem(null);
      await load();
    } catch (err) {
      setActionError(err.message || "Greška prilikom ažuriranja.");
    }
  }

  function handleResetFilters() {
    setKeyword("");
    setSortBy("date");
    setSortDescending(true);

    // pričekaj da React upiše novi state pa onda reload
    setTimeout(() => {
      load();
    }, 0);
  }

  if (loading) return <p>Učitavanje...</p>;
  if (error) return <p style={{ color: "red" }}>Greška: {error}</p>;

  return (
    <div style={{ maxWidth: "900px", margin: "0 auto" }}>
      {/* SEARCH + SORT BAR */}
      <div
        style={{
          marginBottom: "16px",
          padding: "12px",
          border: "1px solid #ddd",
          borderRadius: "6px",
        }}
      >
        <h2>Pretraga i sortiranje oglasa</h2>

        <div
          style={{
            display: "flex",
            flexWrap: "wrap",
            gap: "8px",
            alignItems: "center",
            marginTop: "8px",
          }}
        >
          <input
            type="text"
            placeholder="Pretraži po nazivu/opisu..."
            value={keyword}
            onChange={(e) => setKeyword(e.target.value)}
            style={{ flex: "1 1 200px", padding: "6px" }}
          />

          <select
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
            style={{ padding: "6px" }}
          >
            <option value="date">Datum objave</option>
            <option value="deadline">Rok prijave</option>
            <option value="title">Naslov</option>
          </select>

          <label style={{ display: "flex", alignItems: "center", gap: 4 }}>
            <input
              type="checkbox"
              checked={sortDescending}
              onChange={(e) => setSortDescending(e.target.checked)}
            />
            Silazno
          </label>

          <button type="button" onClick={load}>
            Primijeni
          </button>

          <button type="button" onClick={handleResetFilters}>
            Reset
          </button>
        </div>
      </div>

      {actionError && (
        <p style={{ color: "red" }}>Akcija: {actionError}</p>
      )}

      {internships.map((i) => (
        <div
          key={i.id}
          style={{
            border: "1px solid #ddd",
            marginBottom: "12px",
            borderRadius: "6px",
            padding: "10px",
          }}
        >
          <h2>{i.title}</h2>
          <p>{i.shortDescription}</p>
          <p>
            <strong>Lokacija:</strong> {i.location}
          </p>
          <p>
            <strong>Remote:</strong> {i.remote ? "Da" : "Ne"}
          </p>
          <p>
            <strong>Objavljeno:</strong> {formatDate(i.postedAt)}
          </p>

          {i.deadline && (
            <p>
              <strong>Rok prijave:</strong> {formatDate(i.deadline)}
            </p>
          )}

          <div style={{ display: "flex", gap: 8, marginTop: 10 }}>
            <button onClick={() => setEditItem(i)}>Uredi</button>
            <button onClick={() => handleDelete(i.id)}>Obriši</button>
          </div>

          {editItem?.id === i.id && (
            <EditInternshipForm
              internship={editItem}
              onCancel={() => setEditItem(null)}
              onSave={handleSaveEdit}
            />
          )}
        </div>
      ))}
    </div>
  );
}
