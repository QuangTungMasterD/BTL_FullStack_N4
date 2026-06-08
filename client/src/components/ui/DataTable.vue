<template>
  <div class="overflow-x-auto rounded-xl border border-outline-variant bg-surface-container-lowest">
    <table class="w-full min-w-[640px] border-collapse">
      <thead class="bg-surface-container-low">
        <tr class="border-b border-outline-variant">
          <th
            v-for="col in columns"
            :key="col.key"
            class="px-6 py-4 text-left font-label-md text-label-md text-on-surface-variant uppercase tracking-wider"
            :class="{ 'text-center': col.align === 'center', 'text-right': col.align === 'right' }"
          >
            {{ col.label }}
          </th>
        </tr>
      </thead>
      <tbody class="divide-y divide-outline-variant">
        <tr v-for="row in data" :key="row.id" class="hover:bg-surface-container-low/30 transition-colors">
          <td
            v-for="col in columns"
            :key="col.key"
            class="px-6 py-4 text-body-md text-on-surface"
            :class="{ 'text-center': col.align === 'center', 'text-right': col.align === 'right' }"
          >
            <slot :name="col.key" :row="row">
              {{ row[col.key] }}
            </slot>
          </td>
        </tr>
        <tr v-if="data.length === 0">
          <td :colspan="columns.length" class="px-6 py-8 text-center text-on-surface-variant">
            Không có dữ liệu
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
defineProps({
  columns: { type: Array, required: true }, // [{ key, label, align }]
  data: { type: Array, required: true },
});
</script>