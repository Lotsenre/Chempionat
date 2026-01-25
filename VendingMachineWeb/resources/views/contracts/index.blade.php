@extends('layouts.app')

@section('title', 'Договоры')

@section('content')
<div class="bg-white rounded-lg shadow" x-data="contractsManager()" x-init="init()">
    <!-- Header -->
    <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex items-center justify-between">
            <div>
                <h1 class="text-2xl font-semibold text-gray-800 flex items-center">
                    <i class="fas fa-file-contract text-rose-500 mr-3"></i>
                    Договоры франчайзи
                </h1>
                <p class="text-sm text-gray-500 mt-1">Всего договоров: <span x-text="contracts.length"></span></p>
            </div>
        </div>
    </div>

    <!-- Contracts List -->
    <div class="p-6">
        <!-- Empty State -->
        <template x-if="contracts.length === 0">
            <div class="text-center py-12">
                <div class="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <i class="fas fa-file-contract text-gray-400 text-3xl"></i>
                </div>
                <h3 class="text-lg font-medium text-gray-800 mb-2">Нет договоров</h3>
                <p class="text-gray-500 mb-6">У вас пока нет оформленных договоров франчайзи</p>
                <a href="{{ route('vending-machines.index') }}"
                   class="inline-flex items-center px-5 py-2.5 bg-blue-600 text-white rounded-lg font-medium hover:bg-blue-700 transition">
                    <i class="fas fa-shopping-cart mr-2"></i>
                    Арендовать торговые автоматы
                </a>
            </div>
        </template>

        <!-- Contracts Table -->
        <template x-if="contracts.length > 0">
            <div class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200">
                    <thead>
                        <tr>
                            <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Номер договора</th>
                            <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Дата подписания</th>
                            <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Срок действия</th>
                            <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ТА</th>
                            <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Сумма/мес</th>
                            <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Статус</th>
                            <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Действия</th>
                        </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                        <template x-for="contract in contracts" :key="contract.number">
                            <tr class="hover:bg-gray-50 transition-colors">
                                <td class="px-4 py-4 whitespace-nowrap bg-gray-50/50">
                                    <span class="font-mono font-medium text-gray-900" x-text="contract.number"></span>
                                </td>
                                <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                                    <span x-text="contract.signDate ? formatDate(contract.signDate) : '—'"></span>
                                </td>
                                <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700 bg-gray-50/50">
                                    <div>
                                        <span x-text="formatDate(contract.startDate)"></span>
                                        <span class="text-gray-400 mx-1">—</span>
                                        <span x-text="formatDate(contract.endDate)"></span>
                                    </div>
                                </td>
                                <td class="px-4 py-4 text-sm text-gray-700">
                                    <div class="flex flex-wrap gap-1">
                                        <template x-for="machine in contract.machines" :key="machine.id">
                                            <span class="bg-blue-100 text-blue-800 px-2 py-0.5 rounded text-xs" x-text="machine.name"></span>
                                        </template>
                                    </div>
                                </td>
                                <td class="px-4 py-4 whitespace-nowrap text-sm font-medium text-gray-900 bg-gray-50/50">
                                    <span x-text="formatPrice(contract.monthlyTotal) + ' ₽'"></span>
                                </td>
                                <td class="px-4 py-4 whitespace-nowrap">
                                    <span :class="contract.status === 'signed' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'"
                                          class="px-3 py-1 rounded-full text-xs font-medium">
                                        <i :class="contract.status === 'signed' ? 'fas fa-check-circle' : 'fas fa-clock'" class="mr-1"></i>
                                        <span x-text="contract.status === 'signed' ? 'Подписан' : 'Не подписан'"></span>
                                    </span>
                                </td>
                                <td class="px-4 py-4 whitespace-nowrap text-center bg-gray-50/50">
                                    <div class="flex items-center justify-center gap-2">
                                        <button @click="viewContract(contract)"
                                                class="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition" title="Просмотр">
                                            <i class="fas fa-eye"></i>
                                        </button>
                                        <template x-if="contract.status === 'unsigned'">
                                            <button @click="signContract(contract)"
                                                    class="p-2 text-green-600 hover:bg-green-50 rounded-lg transition" title="Подписать">
                                                <i class="fas fa-file-signature"></i>
                                            </button>
                                        </template>
                                        <template x-if="contract.status === 'signed'">
                                            <button @click="downloadContract(contract)"
                                                    class="p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition" title="Скачать">
                                                <i class="fas fa-download"></i>
                                            </button>
                                        </template>
                                    </div>
                                </td>
                            </tr>
                        </template>
                    </tbody>
                </table>
            </div>
        </template>
    </div>

    <!-- Contract View Modal -->
    <div x-show="showViewModal" x-cloak
         class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center">
            <div class="fixed inset-0 bg-gray-900 bg-opacity-50 transition-opacity" @click="closeViewModal()"></div>

            <div class="relative bg-white rounded-2xl shadow-2xl max-w-4xl w-full mx-auto transform transition-all max-h-[90vh] flex flex-col">
                <!-- Header -->
                <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between flex-shrink-0">
                    <div>
                        <h2 class="text-xl font-bold text-gray-800">Просмотр договора</h2>
                        <p class="text-sm text-gray-500 mt-1">Договор № <span x-text="selectedContract?.number"></span></p>
                    </div>
                    <button @click="closeViewModal()" class="text-gray-400 hover:text-gray-600 p-2">
                        <i class="fas fa-times text-xl"></i>
                    </button>
                </div>

                <!-- Contract Content -->
                <div class="flex-1 overflow-y-auto p-6 bg-gray-100">
                    <div class="bg-white shadow-lg rounded-lg p-8 max-w-3xl mx-auto">
                        <div class="text-center mb-8">
                            <h1 class="text-2xl font-bold text-gray-900">ДОГОВОР ФРАНЧАЙЗИНГА</h1>
                            <p class="text-gray-600 mt-2">№ <span x-text="selectedContract?.number"></span></p>
                            <p class="text-gray-500 text-sm" x-text="selectedContract?.signDate ? 'Подписан: ' + formatDate(selectedContract.signDate) : 'Не подписан'"></p>
                        </div>

                        <div class="space-y-4 text-sm text-gray-700 leading-relaxed">
                            <p><strong>1. ПРЕДМЕТ ДОГОВОРА</strong></p>
                            <p>1.1. Франчайзер предоставляет Франчайзи право на использование торговых автоматов.</p>

                            <p class="mt-6"><strong>2. СРОК ДЕЙСТВИЯ ДОГОВОРА</strong></p>
                            <p>2.1. Договор действует с <span class="font-medium" x-text="formatDate(selectedContract?.startDate)"></span> по <span class="font-medium" x-text="formatDate(selectedContract?.endDate)"></span>.</p>

                            <p class="mt-6"><strong>3. ТОРГОВЫЕ АВТОМАТЫ</strong></p>
                            <div class="ml-4">
                                <template x-for="(machine, index) in selectedContract?.machines || []" :key="machine.id">
                                    <p>3.<span x-text="index + 1"></span>. <span x-text="machine.name"></span> — <span x-text="machine.location"></span></p>
                                </template>
                            </div>

                            <p class="mt-6"><strong>4. СТОИМОСТЬ</strong></p>
                            <p>4.1. Ежемесячная арендная плата: <span class="font-bold" x-text="formatPrice(selectedContract?.monthlyTotal) + ' рублей'"></span>.</p>

                            <p class="mt-6"><strong>5. СПОСОБ ВЕДЕНИЯ</strong></p>
                            <p>5.1. <span x-text="getFranchiseTypeName(selectedContract?.franchiseType)"></span></p>

                            <template x-if="selectedContract?.insurance">
                                <div>
                                    <p class="mt-6"><strong>6. СТРАХОВАНИЕ</strong></p>
                                    <p>6.1. Оборудование застраховано.</p>
                                </div>
                            </template>

                            <!-- Signatures -->
                            <div class="mt-8 pt-6 border-t border-gray-200">
                                <p class="font-medium mb-4">Подписи сторон:</p>
                                <div class="grid grid-cols-2 gap-8">
                                    <div>
                                        <p class="text-gray-500 text-xs mb-2">Франчайзер:</p>
                                        <div class="h-20 border-b-2 border-gray-300 flex items-end pb-1">
                                            <img src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxNTAiIGhlaWdodD0iNTAiPjxwYXRoIGQ9Ik0xMCA0MEMzMCAyMCA1MCAzMCA3MCAyMFM5MCAxMCAxMTAgMzBTMTMwIDIwIDE0MCAzMCIgc3Ryb2tlPSIjMzMzIiBzdHJva2Utd2lkdGg9IjIiIGZpbGw9Im5vbmUiLz48L3N2Zz4=" alt="Подпись" class="h-12">
                                        </div>
                                        <p class="text-xs text-gray-500 mt-1">Директор Иванов И.И.</p>
                                    </div>
                                    <div>
                                        <p class="text-gray-500 text-xs mb-2">Франчайзи:</p>
                                        <div class="h-20 border-b-2 border-gray-300 flex items-end pb-1">
                                            <template x-if="selectedContract?.signature">
                                                <img :src="selectedContract.signature" alt="Подпись" class="h-16">
                                            </template>
                                            <template x-if="!selectedContract?.signature">
                                                <span class="text-gray-400 text-sm">Не подписано</span>
                                            </template>
                                        </div>
                                        <p class="text-xs text-gray-500 mt-1">{{ session('user.name', 'Пользователь') }}</p>
                                    </div>
                                </div>
                            </div>

                            <!-- Encryption Info -->
                            <template x-if="selectedContract?.encryptedHash">
                                <div class="mt-6 p-4 bg-green-50 rounded-lg">
                                    <div class="flex items-center text-green-700">
                                        <i class="fas fa-shield-alt mr-2"></i>
                                        <span class="font-medium">Документ зашифрован и защищён</span>
                                    </div>
                                    <p class="text-xs text-green-600 mt-1 font-mono break-all" x-text="'Hash: ' + selectedContract.encryptedHash.substring(0, 40) + '...'"></p>
                                </div>
                            </template>
                        </div>
                    </div>
                </div>

                <!-- Footer -->
                <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex items-center justify-end rounded-b-2xl">
                    <button @click="closeViewModal()"
                            class="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition">
                        Закрыть
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- Sign Contract Modal -->
    <div x-show="showSignModal" x-cloak
         class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center">
            <div class="fixed inset-0 bg-gray-900 bg-opacity-50 transition-opacity" @click="closeSignModal()"></div>

            <div class="relative bg-white rounded-2xl shadow-2xl max-w-lg w-full mx-auto p-6">
                <div class="text-center mb-6">
                    <div class="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-file-signature text-blue-600 text-2xl"></i>
                    </div>
                    <h3 class="text-xl font-bold text-gray-800">Подписание договора</h3>
                    <p class="text-gray-500 mt-1">Договор № <span x-text="selectedContract?.number"></span></p>
                </div>

                <!-- Signature Pad -->
                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Нарисуйте вашу подпись:</label>
                    <div class="border-2 border-gray-300 rounded-lg overflow-hidden bg-white relative">
                        <canvas id="signaturePadContracts" width="400" height="120" class="w-full cursor-crosshair"></canvas>
                        <button @click="clearSignaturePad()"
                                class="absolute top-2 right-2 text-gray-400 hover:text-gray-600 bg-white rounded-full w-8 h-8 flex items-center justify-center shadow-sm">
                            <i class="fas fa-eraser"></i>
                        </button>
                    </div>
                </div>

                <div class="flex gap-3">
                    <button @click="closeSignModal()"
                            class="flex-1 px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition">
                        Отмена
                    </button>
                    <button @click="confirmSign()"
                            :disabled="isSigningContract"
                            :class="isSigningContract ? 'bg-gray-300 cursor-not-allowed' : 'bg-green-600 hover:bg-green-700'"
                            class="flex-1 px-4 py-2 text-white rounded-lg font-medium transition flex items-center justify-center">
                        <template x-if="isSigningContract">
                            <i class="fas fa-spinner fa-spin mr-2"></i>
                        </template>
                        <span x-text="isSigningContract ? 'Подписание...' : 'Подписать'"></span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

@push('scripts')
<script>
function contractsManager() {
    return {
        contracts: [],
        selectedContract: null,
        showViewModal: false,
        showSignModal: false,
        isSigningContract: false,
        signaturePad: null,

        init() {
            this.loadContracts();
        },

        loadContracts() {
            this.contracts = JSON.parse(localStorage.getItem('contracts') || '[]');
        },

        formatDate(dateStr) {
            if (!dateStr) return '';
            const date = new Date(dateStr);
            return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
        },

        formatPrice(price) {
            return new Intl.NumberFormat('ru-RU').format(price || 0);
        },

        getFranchiseTypeName(type) {
            const types = { 'self': 'Самостоятельное управление', 'managed': 'С поддержкой франчайзера', 'full': 'Полное управление франчайзером' };
            return types[type] || '';
        },

        viewContract(contract) {
            this.selectedContract = contract;
            this.showViewModal = true;
        },

        closeViewModal() {
            this.showViewModal = false;
            this.selectedContract = null;
        },

        signContract(contract) {
            this.selectedContract = contract;
            this.showSignModal = true;
            this.$nextTick(() => this.initSignaturePad());
        },

        closeSignModal() {
            this.showSignModal = false;
            this.selectedContract = null;
        },

        initSignaturePad() {
            const canvas = document.getElementById('signaturePadContracts');
            if (!canvas) return;

            const ctx = canvas.getContext('2d');
            let isDrawing = false;
            let lastX = 0, lastY = 0;

            ctx.strokeStyle = '#1f2937';
            ctx.lineWidth = 2;
            ctx.lineCap = 'round';

            const getPos = (e) => {
                const rect = canvas.getBoundingClientRect();
                const scaleX = canvas.width / rect.width;
                const scaleY = canvas.height / rect.height;
                if (e.touches) {
                    return { x: (e.touches[0].clientX - rect.left) * scaleX, y: (e.touches[0].clientY - rect.top) * scaleY };
                }
                return { x: (e.clientX - rect.left) * scaleX, y: (e.clientY - rect.top) * scaleY };
            };

            const startDrawing = (e) => { isDrawing = true; const pos = getPos(e); lastX = pos.x; lastY = pos.y; };
            const draw = (e) => {
                if (!isDrawing) return;
                e.preventDefault();
                const pos = getPos(e);
                ctx.beginPath();
                ctx.moveTo(lastX, lastY);
                ctx.lineTo(pos.x, pos.y);
                ctx.stroke();
                lastX = pos.x; lastY = pos.y;
            };
            const stopDrawing = () => { isDrawing = false; };

            canvas.addEventListener('mousedown', startDrawing);
            canvas.addEventListener('mousemove', draw);
            canvas.addEventListener('mouseup', stopDrawing);
            canvas.addEventListener('mouseout', stopDrawing);
            canvas.addEventListener('touchstart', startDrawing);
            canvas.addEventListener('touchmove', draw);
            canvas.addEventListener('touchend', stopDrawing);
        },

        clearSignaturePad() {
            const canvas = document.getElementById('signaturePadContracts');
            if (canvas) {
                const ctx = canvas.getContext('2d');
                ctx.clearRect(0, 0, canvas.width, canvas.height);
            }
        },

        async confirmSign() {
            const canvas = document.getElementById('signaturePadContracts');
            if (!canvas) return;

            const signatureData = canvas.toDataURL('image/png');

            this.isSigningContract = true;

            // Simulate signing
            await new Promise(resolve => setTimeout(resolve, 1500));

            // Update contract
            const contractIndex = this.contracts.findIndex(c => c.number === this.selectedContract.number);
            if (contractIndex !== -1) {
                this.contracts[contractIndex].status = 'signed';
                this.contracts[contractIndex].signDate = new Date().toISOString();
                this.contracts[contractIndex].signature = signatureData;
                this.contracts[contractIndex].encryptedHash = btoa(JSON.stringify({
                    contractNumber: this.contracts[contractIndex].number,
                    signDate: this.contracts[contractIndex].signDate
                }));

                localStorage.setItem('contracts', JSON.stringify(this.contracts));
            }

            this.isSigningContract = false;

            if (typeof showToast === 'function') {
                showToast('Договор успешно подписан!', 'success');
            }

            this.closeSignModal();
        },

        downloadContract(contract) {
            // In a real app, this would generate/download a PDF
            if (typeof showToast === 'function') {
                showToast('Скачивание договора...', 'info');
            }

            // Simulate download by opening print dialog
            setTimeout(() => {
                this.viewContract(contract);
                setTimeout(() => {
                    window.print();
                }, 500);
            }, 500);
        }
    };
}
</script>
@endpush
@endsection
