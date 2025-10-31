const form = document.getElementById('form');
const baseUrl = document.getElementById('baseUrl');
const projectId = document.getElementById('projectId');
const tasks = document.getElementById('tasks');
const loading = document.getElementById('loading');
const output = document.getElementById('output');

form.addEventListener('submit', async (e) => {
  e.preventDefault();
  output.textContent = '';
  loading.classList.remove('hidden');

  try {
    const body = JSON.parse(tasks.value);
    const url = `${baseUrl.value.replace(/\/$/, '')}/api/v1/projects/${projectId.value}/schedule`;
    const res = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    });

    if (!res.ok) {
      const text = await res.text();
      throw new Error(text || `Request failed with status ${res.status}`);
    }

    const data = await res.json();
    output.textContent = JSON.stringify(data, null, 2);
  } catch (err) {
    output.textContent = `Error: ${err.message || err}`;
  } finally {
    loading.classList.add('hidden');
  }
});
