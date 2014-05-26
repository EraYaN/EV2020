function loc = location(t)
    c = 340;
    h = 0.28; % Beacon is 28 cm from the ground
    mics = [0 0 0.38; 0 2.90 0.38; 2.90 2.90 0.38; 2.90 0 0.38; -1.05 1.45 0.93];
    
    loc = [];
    points = 0;
    points_std = 0; % Will be overwritten directly since new_points must be larger than 0

    for k = 1:4
        H = 2.9;
        B = 2.9;
        if k == 1
            d1 = t(1) * c; % Base point (0,0)
            d2 = t(4) * c; % Y-direction
            d3 = t(2) * c; % X-direction
        elseif k == 2
            d1 = t(3) * c; % Base point (0,0)
            d2 = t(4) * c; % Y-direction
            d3 = t(2) * c; % X-direction
        elseif k == 3
            d1 = t(2) * c; % Base point (0,0)
            d2 = t(1) * c; % Y-direction
            d3 = t(3) * c; % X-direction
        elseif k == 4
            d1 = t(4) * c; % Base point (0,0)
            d2 = t(3) * c; % Y-direction
            d3 = t(1) * c; % X-direction
        end

        % Calculate the constant time that is added too all arrival times,
        % formula found by Matlab by using report6-2.m
        C = ((B*d1^3 - B^3*d1 + B^3*d2 - H*d1^3 + 2*d1*(((B + d1 - d3)*(B - d1 + d3)*(B^4 - 2*B^3*d1 + 2*B^3*d2 + B^2*H^2 + 2*B^2*H*d1 - 2*B^2*H*d2 + B^2*d1^2 - 2*B^2*d1*d3 - B^2*d2^2 + 2*B^2*d2*d3 - 2*B*H*d1^2 + 4*B*H*d1*d2 - 2*B*H*d2^2 - 2*B*d1^2*d2 + 2*B*d1^2*d3 + 4*B*d1*d2^2 - 4*B*d1*d2*d3 - 2*B*d2^3 + 2*B*d2^2*d3 - H^2*d1^2 + 2*H^2*d1*d3 - H^2*d3^2 + 2*H*d1^2*d2 - 2*H*d1^2*d3 - 4*H*d1*d2^2 + 4*H*d1*d2*d3 + 2*H*d2^3 - 2*H*d2^2*d3 + d1^2*d2^2 - 2*d1^2*d2*d3 + d1^2*d3^2 - 2*d1*d2^3 + 4*d1*d2^2*d3 - 2*d1*d2*d3^2 + d2^4 - 2*d2^3*d3 + d2^2*d3^2))/4)^(1/2) - 2*d2*(((B + d1 - d3)*(B - d1 + d3)*(B^4 - 2*B^3*d1 + 2*B^3*d2 + B^2*H^2 + 2*B^2*H*d1 - 2*B^2*H*d2 + B^2*d1^2 - 2*B^2*d1*d3 - B^2*d2^2 + 2*B^2*d2*d3 - 2*B*H*d1^2 + 4*B*H*d1*d2 - 2*B*H*d2^2 - 2*B*d1^2*d2 + 2*B*d1^2*d3 + 4*B*d1*d2^2 - 4*B*d1*d2*d3 - 2*B*d2^3 + 2*B*d2^2*d3 - H^2*d1^2 + 2*H^2*d1*d3 - H^2*d3^2 + 2*H*d1^2*d2 - 2*H*d1^2*d3 - 4*H*d1*d2^2 + 4*H*d1*d2*d3 + 2*H*d2^3 - 2*H*d2^2*d3 + d1^2*d2^2 - 2*d1^2*d2*d3 + d1^2*d3^2 - 2*d1*d2^3 + 4*d1*d2^2*d3 - 2*d1*d2*d3^2 + d2^4 - 2*d2^3*d3 + d2^2*d3^2))/4)^(1/2) - d1^3*d2 + d1*d3^3 + d1^3*d3 - d2*d3^3 + B^4 - B^2*d1^2 - B^2*d2^2 - B^2*d3^2 + d1^2*d2^2 - 2*d1^2*d3^2 + d2^2*d3^2 + B^2*H*d1 - B^2*H*d2 - B*d1^2*d2 + B^2*d1*d2 + B*d1*d3^2 - 2*B*d1^2*d3 + B^2*d1*d3 - B*d2*d3^2 + B^2*d2*d3 + H*d1^2*d2 - H*d1*d3^2 + 2*H*d1^2*d3 + H*d2*d3^2 + d1*d2*d3^2 - 2*d1*d2^2*d3 + d1^2*d2*d3 + 2*B*d1*d2*d3 - 2*H*d1*d2*d3)/(B^2 - 2*d1^2 + 2*d1*d2 + 2*d1*d3 - d2^2 - d3^2) - B^2 - d1^2 + d2^2)/(2*d1 - 2*d2);
        C = C / c;

        t_corrected = t + real(C);
        [new_loc, new_points, new_points_std] = calc_location(t_corrected, mics, h);

        if new_points > points || (new_points == points && new_points_std < points_std)
            loc = new_loc;
            points = new_points;
            points_std = new_points_std;
        end
    end
end

function [loc, points, points_std] = calc_location(t, mics, h)
    c = 340;
    y = [];
    k = 1;
    for i = 1:size(mics, 1)
        for j = 1:size(mics, 1)
            if i == j || i < j
                continue; % Do not compare with own time or twice thesame
            end

            mic1 = mics(i,:);
            mic2 = mics(j,:);

            t1 = t(i);
            t2 = t(j);

            d = sqrt((mic1(1) - mic2(1))^2 + (mic1(2) - mic2(2))^2);

            h1 = mic1(3) - h;
            h2 = mic2(3) - h;

            % Convert to time in 2D plane
            t1_ = real(sqrt((t1 * c)^2 - h1^2)) / c;
            t2_ = real(sqrt((t2 * c)^2 - h2^2)) / c;

            d1 = ((t1_ * c)^2 - (t2_ * c)^2 + d^2) / (2 * d);
            x = real(sqrt((t1_ * c)^2 - d1^2));
            mx = d1/d * (mic2(1) - mic1(1));
            my = d1/d * (mic2(2) - mic1(2));

            r = [sqrt(mx^2 + my^2), x];
            angle = -atan2(my, mx);
            rot = [cos(angle) -sin(angle); sin(angle) cos(angle)];
            y(k,:) = r * rot + [mic1(1) mic1(2)];
            k = k + 1;

            r = [sqrt(mx^2 + my^2), -x];
            angle = -atan2(my, mx);
            rot = [cos(angle) -sin(angle); sin(angle) cos(angle)];
            y(k,:) = r * rot + [mic1(1) mic1(2)];
            k = k + 1;
        end
    end
    
    y = remove_outliers(y); % Remove the mirror images and other outliers due to input data errors
    loc = mean(y);
    points = size(y,1);
    points_std = std(y(:,1)) + std(y(:,2));
end

% Keep the set of the most close-laying items
function result = remove_outliers(x)
    result = [];
    for i = 1:size(x,1)
        y = [];
        k = 1;
        for j = 1:size(x,1)
            if sum((x(j,:)-x(i,:)).^2) < 0.20^2 % Max 20 cm from the rest
                % Pick the beacon location instead of its mirror image
                firstBest = 0;
                if sum((x(floor((j-1)/2)*2+2,:)-x(i,:)).^2) > sum((x(floor((j-1)/2)*2+1,:)-x(i,:)).^2)    
                    firstBest = 1;
                end
                
                if (mod(j,2) == 1 && firstBest == 1) || (mod(j,2) == 0 && firstBest == 0) % Only get one per two results
                    y(k,:) = x(j,:);
                    k = k + 1;
                end
            end
        end
        if size(y,1) > size(result,1)
            result = y;
        end
    end
end