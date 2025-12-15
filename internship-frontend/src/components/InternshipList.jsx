import { useEffect, useState } from "react";
import { deleteInternship, getInternships, updateInternship } from "../api.js";
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

  const [editItem, setEditItem] = useState(null);
  const [actionError, setActionError] = useState("");

  async function load() {
    setLoading(true);
    setError("");
    try {
      const data = await getInternships();
      setInternships(data);
    } catch (err) {
      setError(err.message || "Dogodila se greška.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function handleDelete(id) {
    setActionError("");

    const ok = window.confirm("Jesi siguran da želiš obrisati ovu praksu?");
    if (!ok) return;

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

  if (loading) return <p>Učitavanje...</p>;
  if (error) return <p style={{ color: "red" }}>Greška: {error}</p>;

  return (
    <div style={{ maxWidth: "900px", margin: "0 auto" }}>
      {actionError && <p style={{ color: "red" }}>Akcija: {actionError}</p>}

      {internships.map(i => (
        <div
          key={i.id}
          style={{
            padding: "16px",
            border: "1px solid #ddd",
            marginBottom: "12px",
            borderRadius: "6px"
          }}
        >
          <h2>{i.title}</h2>
          <p>{i.shortDescription}</p>
          <p><strong>Lokacija:</strong> {i.location}</p>
          <p><strong>Remote:</strong> {i.remote ? "Da" : "Ne"}</p>
          <p><strong>Objavljeno:</strong> {formatDate(i.postedAt)}</p>

          {i.deadline && (
            <p><strong>Rok prijave:</strong> {formatDate(i.deadline)}</p>
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
