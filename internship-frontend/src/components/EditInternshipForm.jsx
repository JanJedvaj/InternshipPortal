import { useEffect, useState } from "react";

export default function EditInternshipForm({ internship, onCancel, onSave }) {
  const [form, setForm] = useState(internship);

  useEffect(() => {
    setForm(internship);
  }, [internship]);

  if (!form) return null;

  function updateField(name, value) {
    setForm(prev => ({ ...prev, [name]: value }));
  }

  function handleSubmit(e) {
    e.preventDefault();
    onSave(form);
  }

  return (
    <form onSubmit={handleSubmit} style={{ padding: 12, border: "1px solid #eee", borderRadius: 6, marginTop: 12 }}>
      <h3>Uredi praksu</h3>

      <div style={{ marginBottom: 8 }}>
        <label>Naslov</label><br />
        <input
          value={form.title || ""}
          onChange={(e) => updateField("title", e.target.value)}
          style={{ width: "100%", padding: 8 }}
        />
      </div>

      <div style={{ marginBottom: 8 }}>
        <label>Kratki opis</label><br />
        <input
          value={form.shortDescription || ""}
          onChange={(e) => updateField("shortDescription", e.target.value)}
          style={{ width: "100%", padding: 8 }}
        />
      </div>

      <div style={{ marginBottom: 8 }}>
        <label>Lokacija</label><br />
        <input
          value={form.location || ""}
          onChange={(e) => updateField("location", e.target.value)}
          style={{ width: "100%", padding: 8 }}
        />
      </div>

      <div style={{ marginBottom: 8 }}>
        <label>Remote</label>{" "}
        <input
          type="checkbox"
          checked={!!form.remote}
          onChange={(e) => updateField("remote", e.target.checked)}
        />
      </div>

      <div style={{ display: "flex", gap: 8 }}>
        <button type="submit">Spremi</button>
        <button type="button" onClick={onCancel}>Odustani</button>
      </div>
    </form>
  );
}
