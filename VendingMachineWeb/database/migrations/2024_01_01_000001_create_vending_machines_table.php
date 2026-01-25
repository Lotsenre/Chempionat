<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('vending_machines', function (Blueprint $table) {
            $table->uuid('id')->primary();
            $table->string('serial_number', 50)->unique();
            $table->string('name', 255);
            $table->string('model', 255);
            $table->string('status', 50)->default('Working');
            $table->string('location', 500);
            $table->string('place', 500)->nullable();
            $table->string('coordinates', 100)->nullable();
            $table->timestamp('install_date');
            $table->timestamp('last_maintenance_date')->nullable();
            $table->string('working_hours', 50)->nullable();
            $table->string('timezone', 50)->nullable();
            $table->decimal('total_income', 18, 2)->default(0);
            $table->uuid('user_id')->nullable();
            $table->string('work_mode', 100)->nullable();
            $table->string('rfid_cash_collection', 50)->nullable();
            $table->string('rfid_loading', 50)->nullable();
            $table->string('rfid_service', 50)->nullable();
            $table->string('kit_online_id', 50)->nullable();
            $table->string('company', 255)->nullable();
            $table->string('payment_type', 255)->nullable();
            $table->string('critical_threshold_template', 100)->nullable();
            $table->string('service_priority', 50)->nullable();
            $table->string('manager', 255)->nullable();
            $table->string('notification_template', 100)->nullable();
            $table->string('engineer', 255)->nullable();
            $table->string('operator', 255)->nullable();
            $table->string('technician', 255)->nullable();
            $table->text('notes')->nullable();
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('vending_machines');
    }
};
