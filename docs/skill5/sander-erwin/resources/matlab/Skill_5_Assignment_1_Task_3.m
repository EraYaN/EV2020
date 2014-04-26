L0 = 0.5e-6;
C0 = 2e-12;
Zl = 100;
Vmax = 1;
tw = 0.5e-9;
l = 1.2;

Z0 = sqrt(L0/C0);
Gamma = (Zl - Z0) / (Zl + Z0);
gamma = sqrt(C0 * L0);

t = 0:0.05e-9:5e-9;
x = 0:0.012:1.2;

V = zeros(length(x), length(t));
I = zeros(length(x), length(t));

U = @(t) Vmax * (heaviside(t) - heaviside(t - tw));

for i = 1:length(x)
    for j = 1:length(t)
         V(i,j) =            U(t(j) - gamma * x(i)) ...
                + Gamma *   U(t(j) + gamma * (x(i) - 2 * l)) ...
                - Gamma *   U(t(j) - gamma * (x(i) + 2 * l)) ...
                - Gamma.^2 *U(t(j) + gamma * (x(i) - 4 * l)) ...
                + Gamma.^2 *U(t(j) - gamma * (x(i) + 4 * l));
        I(i,j) = V(i,j)/Zl;
    end
end

Bscan_plot(t, x, V, 't (s)', 'x (m)', 'U (V)', [20 45])
%Bscan_plot(t, x, I, 't (s)', 'x (m)', 'I (A)', [20 45])