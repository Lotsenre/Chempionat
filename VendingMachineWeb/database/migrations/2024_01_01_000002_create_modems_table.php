<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('modems', function (Blueprint $table) {
            $table->uuid('id')->primary();
            $table->string('modem_number', 50)->unique();
            $table->string('model', 100)->nullable();
            $table->string('status', 50)->default('Active');
            $table->uuid('vending_machine_id')->nullable();
            $table->timestamps();

            $table->foreign('vending_machine_id')
                ->references('id')
                ->on('vending_machines')
                ->onDelete('set null');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('modems');
    }
};
